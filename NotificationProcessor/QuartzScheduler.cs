using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Xml.JobSchedulingData20;
using ROHV.Core.Database;

namespace NotificationProcessor
{
    public static class QuartzScheduler
    {
        private static readonly StdSchedulerFactory _factory;
        private static readonly NameValueCollection _configuration;
        private static IScheduler scheduler;


        static QuartzScheduler() {
            _factory = new StdSchedulerFactory();
            scheduler = _factory.GetScheduler().Result;
        }

        public static async Task StartScheduler() => await scheduler.Start();

        public static void JobDisplayer() {
            var jobGroups = scheduler.GetJobGroupNames().Result;
            // IList<string> triggerGroups = scheduler.GetTriggerGroupNames();

            foreach (string group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = scheduler.GetJobKeys(groupMatcher).Result;
                foreach (var jobKey in jobKeys)
                {
                    Console.WriteLine("Job: " + jobKey.Name);
                    var detail = scheduler.GetJobDetail(jobKey).Result;
                    var triggers = scheduler.GetTriggersOfJob(jobKey).Result;
                    foreach (ITrigger trigger in triggers)
                        
                    {
                        Console.WriteLine("\tTrigger: " + trigger.Key.Name);
                        Console.WriteLine("\tDescription: " + trigger.Description);
                        DateTimeOffset? nextFireTime = trigger.GetNextFireTimeUtc();
                        if (nextFireTime.HasValue)
                        {
                            Console.WriteLine("\t\tNext fire: " + nextFireTime.Value.LocalDateTime.ToString());
                        }

                        DateTimeOffset? previousFireTime = trigger.GetPreviousFireTimeUtc();
                        if (previousFireTime.HasValue)
                        {
                            Console.WriteLine("\t\tPrevious fire: " + previousFireTime.Value.LocalDateTime.ToString());
                        }
                    }
                    Console.WriteLine();
                }
            } 
        }
        
        public static async Task ScheduleJobs(IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> triggersAndJobs) {
            Console.WriteLine("____ScheduleJobs_____");
            await scheduler.ScheduleJobs(triggersAndJobs, replace: false);
            JobDisplayer();
        }

        public static async Task ScheduleJob(IJobDetail jobDetail, ITrigger trigger) {
            Console.WriteLine("____ScheduleJob_____");
            await scheduler.ScheduleJob(jobDetail, trigger);
            JobDisplayer();
        }

        public static async Task DetachTrigger(TriggerKey triggerKey) {
            Console.WriteLine("____DetachTrigger_____");
            await scheduler.UnscheduleJob(triggerKey);
            JobDisplayer();
        }

        public static async Task AttachTrigger(ITrigger trigger) {
            Console.WriteLine("____AttachTrigger_____");
            if (trigger.JobKey is null) {
                throw new Exception("The trigger must have reference to appropriate job.\n Configure method <ForJob> in the TriggerBuilder");
            }
            
            var jobKey = trigger.JobKey;
            var existJobKey = await scheduler.CheckExists(jobKey);
            if (existJobKey) 
                await scheduler.ScheduleJob(trigger);    
            else {
                var repeatTypes = Enum.GetNames(typeof(Repeat)).ToList();
                var pattern = string.Join("|", repeatTypes);
                var foundRepeatType = Regex.Match(jobKey.ToString(), pattern).Value;
                if (foundRepeatType == string.Empty) {
                    throw new Exception($"The job not found and the job key doesn't match to the declared job types: {jobKey} ");
                }

                var repeatType = repeatTypes.IndexOf(foundRepeatType) + 1;
                var jobDetail = QuartzJob.CreateJob((Repeat) repeatType);
                await ScheduleJob(jobDetail, trigger);
            }
            
            JobDisplayer();
        }

//        public static async Task UpdateTrigger(TriggerKey triggerKey, ITrigger newTrigger) {
//            Console.WriteLine("____UpdateTrigger_____");
//            await scheduler.RescheduleJob(triggerKey, newTrigger);
//            JobDisplayer();
//        }
    }

    public static class QuartzTrigger
    {
        private const string TriggerPrefixName = "trigger_";
        private const string TriggersPrefixName = "triggers_";
        public static ITrigger CreateTrigger(DateTime startDate, Repeat repeatType, string description, string cronExpression) {
            var triggerKey = TriggerPrefixName + startDate;
            var groupKey = TriggersPrefixName + repeatType;
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey, groupKey)
                .WithDescription(description)
                .WithCronSchedule(cronExpression)
                .Build();
            return trigger;
        }

        public static ITrigger CreateTrigger(DateTime startDate, string description) {
            var triggerKey = TriggerPrefixName + startDate;
            var groupKey = TriggersPrefixName + Repeat.Once;
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey, groupKey)
                .WithDescription(description)
                .StartAt(startDate)
                .Build();
            return trigger;
        }

        public static ITrigger CreateTrigger(DateTime startDate, Repeat repeatType, string description) {
            ITrigger trigger;
            if (repeatType == Repeat.Once)
                trigger = CreateTrigger(startDate, description);
            else {
                var cronExpression = CronJob.GetCronExpression(repeatType, startDate);
                var triggerKey = TriggerPrefixName + startDate;
                var groupKey = TriggersPrefixName + repeatType;
                trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey, groupKey)
                    .WithCronSchedule(cronExpression)
                    .WithDescription(description)
                    .Build();
            }
            return trigger;
        }
        
        public static ITrigger CreateTrigger(Repeat repeatType, string description = "") {
            
            var cronExpression = CronJob.GetCronExpression(repeatType, default);
            var triggerKey = TriggerPrefixName + repeatType;
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithCronSchedule(cronExpression)
                .WithDescription(description)
                .StartNow()
                .Build();
            return trigger;
        }

        public static ITrigger CreateTriggerForExistedJob(DateTime startDate, string description = "") {
            var triggerKey = TriggerPrefixName + startDate;
            var groupKey = TriggersPrefixName + Repeat.Once;
            var jobKey = QuartzJob.GetJobKey(Repeat.Once);
            return TriggerBuilder.Create()
                .WithIdentity(triggerKey, groupKey)
                .WithDescription(description)
                .ForJob(jobKey)
                .Build();
        }
        
        public static ITrigger CreateTriggerForExistedJob(DateTime startDate, Repeat repeatType, string description = "") {
            if (repeatType == Repeat.Once) {
                return CreateTriggerForExistedJob(startDate, description);
            }
            var cronExpression = CronJob.GetCronExpression(repeatType, startDate);
            var triggerKey = TriggerPrefixName + startDate;
            var triggersGroupKey = TriggersPrefixName + repeatType;
            var jobKey = QuartzJob.GetJobKey(repeatType);
            return TriggerBuilder.Create()
                .WithIdentity(triggerKey, triggersGroupKey)
                .WithCronSchedule(cronExpression)
                .WithDescription(description)
                .ForJob(jobKey)
                .Build();
        }

        public static TriggerKey GetTriggerKey(DateTime startDate, Repeat repeatType) {
            var triggerKey = TriggerPrefixName + startDate;
            var groupKey = TriggersPrefixName + repeatType;
            return new TriggerKey(triggerKey, groupKey);
        }
    }

    public static class QuartzJob
    {
        private const string JobPrefixName = "job_";
        private const string JobsPrefixName = "jobs_";
        
        public static IJobDetail CreateJob(Repeat repeatType) {
            Type jobType;
            switch (repeatType) {
                case Repeat.Once: jobType = typeof(OnceFiredEmailNotificationsJob);
                    break;
                case Repeat.EveryMinute: jobType = typeof(UpdateNotificationJobsJob);
                    break;
                default: jobType = typeof(RepeatedEmailNotificationsJob);
                    break;
            }

            var jobKey = JobPrefixName + repeatType;
            var groupKey = JobsPrefixName + repeatType;
            var jobDetail = JobBuilder.Create(jobType)
                .WithIdentity(jobKey, groupKey)
                .StoreDurably()
                .Build();
            return jobDetail;
        }

        public static JobKey GetJobKey(Repeat repeatType) => new JobKey(JobPrefixName + repeatType, JobsPrefixName + repeatType);
    }
}