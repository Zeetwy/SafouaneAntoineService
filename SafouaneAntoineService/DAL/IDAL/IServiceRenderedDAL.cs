﻿using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.DAL.IDAL
{
    public interface IServiceRenderedDAL
    {
        public bool ConfirmService(ServiceRendered service);
        public bool ValidateService(ServiceRendered service);
    }
}
