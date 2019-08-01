using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NotificationProcessor.Quartz.Jobs;
using Quartz;
using Quartz.Impl;

namespace NotificationProcessor.Quartz
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
        
        public static async Task ScheduleJobs(IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> triggersAndJobs) {
            await scheduler.ScheduleJobs(triggersAndJobs, replace: false);
        }

        public static async Task ScheduleJob(IJobDetail jobDetail, ITrigger trigger) {
            await scheduler.ScheduleJob(jobDetail, trigger);
        }

        public static async Task DetachTrigger(TriggerKey triggerKey) {
            await scheduler.UnscheduleJob(triggerKey);
        }

        public static async Task AttachTrigger(ITrigger trigger) {
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
        }
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