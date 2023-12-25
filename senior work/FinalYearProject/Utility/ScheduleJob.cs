using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;
using FinalYearProject.Utility;

namespace FinalYearProject.Utility
{
    public class ScheduleJob : CronJobService
    {
        private readonly ILogger<ScheduleJob> _logger;
        private readonly IServiceScopeFactory _sFactory;

        public ScheduleJob(IServiceScopeFactory scopeFactory, IScheduleConfig<ScheduleJob> config, ILogger<ScheduleJob> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _sFactory = scopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            initializeRole();
            updateLeaveAccrual();

            _logger.LogInformation("ScheduleJob starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            updateLeaveAccrual();

            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ScheduleJob is working.");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ScheduleJob is stopping.");
            return base.StopAsync(cancellationToken);
        }

        public async void initializeRole()
        {
            using (var scope = _sFactory.CreateScope())
            {
                var _roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                //Admin

                var superAdmin = await _roleManager.FindByNameAsync(SD.SuperAdmin);

                if (superAdmin == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdmin));
                }

                var admin = await _roleManager.FindByNameAsync(SD.Admin);

                if (admin == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Admin));
                }

                //Staff

                var employeeView = await _roleManager.FindByNameAsync(SD.EmployeeView);

                if (employeeView == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.EmployeeView));
                }

                var employeeRegister = await _roleManager.FindByNameAsync(SD.EmployeeRegister);

                if (employeeRegister == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.EmployeeRegister));
                }

                var employeeManage = await _roleManager.FindByNameAsync(SD.EmployeeManage);

                if (employeeManage == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.EmployeeManage));
                }

                var roleManage = await _roleManager.FindByNameAsync(SD.RoleManage);

                if (roleManage == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.RoleManage));
                }

                var shiftManage = await _roleManager.FindByNameAsync(SD.ShiftManage);

                if (shiftManage == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.ShiftManage));
                }

                var trainingManage = await _roleManager.FindByNameAsync(SD.TrainingManage);

                if (trainingManage == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.TrainingManage));
                }

                var payrateManage = await _roleManager.FindByNameAsync(SD.PayrateManage);

                if (payrateManage == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.PayrateManage));
                }
            }
        }

        public async void updateLeaveAccrual()
        {
            using (var scope = _sFactory.CreateScope())
            {
                var _db = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var companies = await _db.Company.ToListAsync();

                foreach (var company in companies)
                {
                    var employees = await _db.EmployeeDetails.Where(e => e.parent_company == company.company_id).ToListAsync();

                    int hoursInADay = company.leaveHoursPerDay;
                    int paidLeaveCarryOver = company.paidMaxCarryover * hoursInADay;
                    int paidLeavePerYear = company.paidLeaveYearly * hoursInADay;
                    int sickLeaveCarryOver = company.sickMaxCarryover * hoursInADay;
                    int sickLeavePerYear = company.sickLeaveYearly * hoursInADay;

                    foreach (var employee in employees)
                    {
                        if (employee.leaveUpdate.Year < DateTime.Now.Year)
                        {
                            var paidLeaveLeft = employee.paidLeaveHourLeft + employee.paidLeaveOnBargain;

                            if (paidLeaveLeft > paidLeaveCarryOver)
                            {
                                paidLeaveLeft = paidLeaveCarryOver;
                            }

                            var newPaidLeave = paidLeaveLeft + paidLeavePerYear;

                            newPaidLeave -= employee.paidLeaveOnBargain;

                            employee.paidLeaveHourLeft = newPaidLeave;


                            var sickLeaveLeft = employee.sickLeaveHourLeft + employee.sickLeaveOnBargain;

                            if (sickLeaveLeft > sickLeaveCarryOver)
                            {
                                sickLeaveLeft = sickLeaveCarryOver;
                            }

                            var newSickLeave = sickLeaveLeft + sickLeavePerYear;

                            newSickLeave -= employee.sickLeaveOnBargain;

                            employee.sickLeaveHourLeft = newSickLeave;

                            employee.leaveUpdate = DateTime.Now;

                            _db.Update(employee);
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}
