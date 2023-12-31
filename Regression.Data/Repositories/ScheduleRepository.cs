﻿using Regression.Data.Interfaces;
using Regression.Domain;
using Regression.Domain.Entities;

namespace Regression.Data.Repositories
{
    public class ScheduleRepository : GeneralRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(RegressionContext context) : base(context)
        { }
    }
}