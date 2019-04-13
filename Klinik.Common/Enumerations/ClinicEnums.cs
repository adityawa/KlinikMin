namespace Klinik.Common
{
    /// <summary>
    /// Class contains collection of enumeration
    /// </summary>
    public class ClinicEnums
    {
        public enum Status
        {
            SUCCESS,
            ERROR,
            UNRECOGNIZED
        }

        public enum MasterTypes
        {
            EmploymentType,
            Department,
            City,
            ClinicType,
            DoctorType,
            PoliScheduleStatus,
            ParamedicType,
            Day,
            PoliType
        }

        public enum AuthResult
        {
            SUCCESS,
            UNRECOGNIZED
        }

        public enum Module
        {
            LOGIN,
            MASTER_ORGANIZATION,
            MASTER_ORGANIZATION_PRIVILEGE,
            MASTER_CLINIC,
            MASTER_EMPLOYEE,
            MASTER_PRIVILEGE,
            MASTER_ROLE,
            MASTER_ROLE_PRIVILEGE,
            MASTER_USER,
            MASTER_USER_ROLE,
            MASTER_DOCTOR,
            EMPLOYEE_ASSIGNMENT,
            REGISTRATION,
            POLI_SCHEDULE,
            POLI_SCHEDULE_MASTER,
            MASTER_PARAMEDIC,
            Patient,
            FormPreExamine,
            MASTER_POLI,
            MASTER_POLI_CLINIC
        }

        public enum Action
        {
            Add,
            Edit,
            DELETE,
            Process,
            Hold,
            Finish,
            Reschedule
        }

        public enum SourceTable
        {
            PATIENT
        }

    }
}