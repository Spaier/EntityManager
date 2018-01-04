using EntityManager.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace EntityManager.AspNetCore.Authorization
{
    public static class CrudOperations
    {
        public static OperationAuthorizationRequirement Create =
          new OperationAuthorizationRequirement { Name = CrudOperationsNames.CreateOperationName };

        public static OperationAuthorizationRequirement Read =
          new OperationAuthorizationRequirement { Name = CrudOperationsNames.ReadOperationName };

        public static OperationAuthorizationRequirement Update =
          new OperationAuthorizationRequirement { Name = CrudOperationsNames.UpdateOperationName };

        public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement { Name = CrudOperationsNames.DeleteOperationName };
    }
}
