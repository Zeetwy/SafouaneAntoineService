﻿using SafouaneAntoineService.Models;
using SafouaneAntoineService.ViewModels;

namespace SafouaneAntoineService.DAL.IDAL
{
    public interface IServiceOfferDAL
    {
        public List<ServiceOffer> GetServices();
        ServiceOffer GetService(int id);

        public List<ServiceOffer> GetOffersByUser(User user);
        public bool PublishOffer(User user, ServiceOffer so);
    }
}
