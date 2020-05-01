﻿using System;
using System.Net.Http;
using System.Text;
using Microsoft.Xrm.Sdk;
using AN.Integration.Dynamics.EntityProviders;
using AN.Integration.Dynamics.Extensions;
using AN.Integration.Dynamics.Models;
using AN.Integration.DynamicsCore.CoreTypes;
using AN.Integration.DynamicsCore.Utilities;

namespace AN.Integration.Sender.Messages
{
    public class Send : IPlugin
    {
        #region Secure/Unsecure Configuration Setup

        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public Send(string unsecureConfig, string secureConfig)
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }

        public Send()
        {
        }

        #endregion

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);

            try
            {
                var settings = new Settings(service).GetSettings(
                    SBCustomSettingsModel.Fields.ServiceBusExportQueueuUrl,
                    SBCustomSettingsModel.Fields.ServiceBusExportQueueuSasKey);

                settings.EnsureParameterIsSet(nameof(settings.ServiceBusExportQueueuUrl));
                settings.EnsureParameterIsSet(nameof(settings.ServiceBusExportQueueuSasKey));

                var contextCore = new DynamicsContextCore
                {
                    MessageType = (ContextMessageType)
                        Enum.Parse(typeof(ContextMessageType), context.MessageName),
                    UserId = context.UserId,
                    InputParameters = context.InputParameters.ToCollectionCore(),
                    PreEntityImages = context.PreEntityImages.ToCollectionCore(),
                    PostEntityImages = context.PostEntityImages.ToCollectionCore()
                };

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", settings.ServiceBusExportQueueuSasKey);

                var result = httpClient.PostAsync(new Uri(settings.ServiceBusExportQueueuUrl),
                    new StringContent(ContextSerializer.ToJson(contextCore),
                        Encoding.UTF8, "application/json")).GetAwaiter().GetResult();

                if (!result.IsSuccessStatusCode)
                {
                    var response = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    throw new Exception($"Send message error:\n {response}");
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}