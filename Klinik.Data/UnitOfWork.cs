using Klinik.Data.DataRepository;
using System;

namespace Klinik.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly klinikEntities _context;
        private IGenericRepository<Clinic> _clinicRepository;
        private IGenericRepository<Organization> _organizationRepository;
        private IGenericRepository<Privilege> _privilegeRepository;
        private IGenericRepository<OrganizationRole> _roleRepository;
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<Employee> _employeeRepository;
        private IGenericRepository<GeneralMaster> _masterRepository;
        private IGenericRepository<OrganizationPrivilege> _orgprivRepository;
        private IGenericRepository<RolePrivilege> _roleprivRepository;
        private IGenericRepository<UserRole> _userroleRepository;
        private IGenericRepository<Menu> _menuRepository;
        private IGenericRepository<PasswordHistory> _passwordHistRepostiory;
        private IGenericRepository<Log> _logRepository;
        private IGenericRepository<EmployeeAssignment> _employeeAssignmentRepository;
        private IGenericRepository<EmployeeStatu> _employeeStatusRepository;
        public IGenericRepository<FamilyRelationship> _familyRelationshipRepository;
       
        private IGenericRepository<FileArchieve> _fileArchiveRepository;
        private IGenericRepository<City> _cityRepository;
        
        private bool disposed = false;

        public UnitOfWork(klinikEntities context)
        {
            _context = context;
        }

       

        public IGenericRepository<Organization> OrganizationRepository
        {
            get
            {
                if (_organizationRepository == null)
                    _organizationRepository = new GenericRepository<Organization>(_context);
                return _organizationRepository;
            }
        }

        public IGenericRepository<Clinic> ClinicRepository
        {
            get
            {
                if (_clinicRepository == null)
                    _clinicRepository = new GenericRepository<Clinic>(_context);
                return _clinicRepository;
            }
        }

        public IGenericRepository<Privilege> PrivilegeRepository
        {
            get
            {
                if (_privilegeRepository == null)
                    _privilegeRepository = new GenericRepository<Privilege>(_context);
                return _privilegeRepository;
            }
        }

        public IGenericRepository<OrganizationRole> RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                    _roleRepository = new GenericRepository<OrganizationRole>(_context);
                return _roleRepository;
            }
        }

        public IGenericRepository<User> UserRepository
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new GenericRepository<User>(_context);
                return _userRepository;
            }
        }

        public IGenericRepository<Employee> EmployeeRepository
        {
            get
            {
                if (_employeeRepository == null)
                    _employeeRepository = new GenericRepository<Employee>(_context);

                return _employeeRepository;
            }
        }

        public IGenericRepository<City> CityRepository
        {
            get
            {
                if (_cityRepository == null)
                    _cityRepository = new GenericRepository<City>(_context);

                return _cityRepository;
            }
        }

        public IGenericRepository<GeneralMaster> MasterRepository
        {
            get
            {
                if (_masterRepository == null)
                    _masterRepository = new GenericRepository<GeneralMaster>(_context);

                return _masterRepository;
            }
        }

        public IGenericRepository<OrganizationPrivilege> OrgPrivRepository
        {
            get
            {
                if (_orgprivRepository == null)
                    _orgprivRepository = new GenericRepository<OrganizationPrivilege>(_context);

                return _orgprivRepository;
            }
        }

        public IGenericRepository<RolePrivilege> RolePrivRepository
        {
            get
            {
                if (_roleprivRepository == null)
                    _roleprivRepository = new GenericRepository<RolePrivilege>(_context);

                return _roleprivRepository;
            }
        }

        public IGenericRepository<UserRole> UserRoleRepository
        {
            get
            {
                if (_userroleRepository == null)
                    _userroleRepository = new GenericRepository<UserRole>(_context);

                return _userroleRepository;
            }
        }

        public IGenericRepository<Menu> MenuRepository
        {
            get
            {
                if (_menuRepository == null)
                    _menuRepository = new GenericRepository<Menu>(_context);

                return _menuRepository;
            }
        }

        public IGenericRepository<PasswordHistory> PasswordHistoryRepository
        {
            get
            {
                if (_passwordHistRepostiory == null)
                    _passwordHistRepostiory = new GenericRepository<PasswordHistory>(_context);

                return _passwordHistRepostiory;
            }
        }

        public IGenericRepository<Log> LogRepository
        {
            get
            {
                if (_logRepository == null)
                    _logRepository = new GenericRepository<Log>(_context);

                return _logRepository;
            }
        }

        public IGenericRepository<EmployeeAssignment> EmployeeAssignmentRepository
        {
            get
            {
                if (_employeeAssignmentRepository == null)
                    _employeeAssignmentRepository = new GenericRepository<EmployeeAssignment>(_context);

                return _employeeAssignmentRepository;
            }
        }

        public IGenericRepository<EmployeeStatu> EmployeeStatusRepository
        {
            get
            {
                if (_employeeStatusRepository == null)
                    _employeeStatusRepository = new GenericRepository<EmployeeStatu>(_context);

                return _employeeStatusRepository;
            }
        }

        public IGenericRepository<FamilyRelationship> FamilyRelationshipRepository
        {
            get
            {
                if (_familyRelationshipRepository == null)
                    _familyRelationshipRepository = new GenericRepository<FamilyRelationship>(_context);

                return _familyRelationshipRepository;
            }
        }

       

        public IGenericRepository<FileArchieve> FileArchiveRepository
        {
            get
            {
                if (_fileArchiveRepository == null)
                    _fileArchiveRepository = new GenericRepository<FileArchieve>(_context);

                return _fileArchiveRepository;
            }
        }

      

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        int IUnitOfWork.Save()
        {
            return _context.SaveChanges();
        }
    }
}
