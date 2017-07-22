using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace SnapShop
{
    public class Azure
    {

        private static Azure instance;
        private MobileServiceClient client;
        private IMobileServiceTable<SnapShopNZInformation> DataTable;

        private Azure()
        {
            this.client = new MobileServiceClient("http://snapshopnz.azurewebsites.net");
            this.DataTable = this.client.GetTable<SnapShopNZInformation>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static Azure AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Azure();
                }

                return instance;
            }
        }

        public async Task<List<SnapShopNZInformation>> GetHotDogInformation()
        {
            return await this.DataTable.ToListAsync();
        }

        public async Task PostHotDogInformation(SnapShopNZInformation SnapShopModel)
        {
            await this.DataTable.InsertAsync(SnapShopModel);
        }

    }
}
