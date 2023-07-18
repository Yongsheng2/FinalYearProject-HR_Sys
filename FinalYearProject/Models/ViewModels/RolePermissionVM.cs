using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models.ViewModels
{
    public class RolePermissionVM
    {
        public Role Role { get; set; }

        [Display(Name = "Employee Related Functions")]
        public bool employeeView { get; set; }

        [Display(Name = "Create New Employee Account")]
        public bool employeeRegister { get; set; }

        [Display(Name = "Workforce Management")]
        public bool employeeManage { get; set; }

        [Display(Name = "Role Management")]
        public bool roleManage { get; set; }

        [Display(Name = "Shift Management")]
        public bool shiftManage { get; set; }

        [Display(Name = "Training Management")]
        public bool trainingManage { get; set; }

        [Display(Name = "Payrate Management")]
        public bool payrateManage { get; set; }

        public RolePermissionVM()
        {

        }

        public RolePermissionVM(Role role) 
        {
            Role = role;
            employeeView = true;
            employeeRegister = false;
            employeeManage = false;
            roleManage = false;
            shiftManage = false;
            trainingManage = false;
            payrateManage = false;
        }
    }
}
