using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace NotificationProcessor
{
    public static class QuartzScheduler
    {
        private static readonly StdSchedulerFactory _factory;
        private static readonly NameValueCollection _configuration;
        private static IScheduler scheduler;

        static QuartzScheduler() {
            _configuration = new NameValueCollection() {
                { "quartz.serializer.type", "binary" }
            };
            _factory = new StdSchedulerFactory(_configuration);
            scheduler = _factory.GetScheduler().Result;
        }

        public static async Task StartScheduler() => await scheduler.Start();

        public static async Task ScheduleJobs(
            IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> triggersAndJobs) =>
            await scheduler.ScheduleJobs(triggersAndJobs, replace: false);

        public static async Task ScheduleJob(IJobDetail jobDetail, ITrigger trigger) =>
            await scheduler.ScheduleJob(jobDetail, trigger);
    }

    public static class QuartzTrigger
    {
        public static ITrigger CreateTrigger(string triggerName, string groupName, string cronExpression) =>
            TriggerBuilder.Create()
                .WithIdentity(triggerName, groupName)
                .WithCronSchedule(cronExpression)
                .Build();

        public static ITrigger CreateTrigger(string triggerName, string groupName, DateTime startDate) =>
            TriggerBuilder.Create()
                .WithIdentity(triggerName, groupName)
                .StartAt(startDate)
                .Build();
    }

    public static class QuartzJob
    {
        public static IJobDetail CreateJob<T>(string jobName, string groupName) where T : IJob =>
            JobBuilder.Create<T>()
                .WithIdentity(jobName, groupName)
                .Build();
    }
}