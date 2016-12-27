﻿using System;
using System.Configuration;
using Newtonsoft.Json;
using PayStack.Net;
using System.IO;
using System.Reflection;
using PayStack.Net.Apis;

namespace test_console
{
    internal class Program
    {
        private static PayStackApi _api;

        private static void Main(string[] args)
        {
            _api = new PayStackApi(ConfigurationManager.AppSettings["PayStackSecret"]);

            //
            // Sub Accounts
            //
            // ListSubAccounts();
            // UpdateSubAccount();
            GetBanks();

            //
            // Customers
            //
            // CustomersList();
            // CustomerFetch();
            // CustomerUpdate();
            //CustomerRiskAction();

            //
            // Transactions
            //
            // TransactionExport_Setup();
            //TransactionTotals_Setup();
            //TransactionTimeline_Setup();
            // TransactionFetch_Setup();
            // TransactionList_Setup();
            // InitializeRequest_Setup();
        }

        private static void GetBanks()
        {
            _api.SubAccounts.GetBanks().Print();
        }

        private static void UpdateSubAccount()
        {
            var suba = _api.SubAccounts.Fetch("ACCT_v1wico0y3742ecn");

            if (!suba.Status) return;

            // Populate sub account request from fetched object
            var request = new SubAccountUpdateRequest().PopulateWith(suba);

            // Update as necessary
            request.BusinessName = "NMA 2017 Conference Account (Updated)";

            // Call the API
            var response = _api.SubAccounts.Update("ACCT_v1wico0y3742ecn", request);

            Console.WriteLine(response.Status ? "Subaccount successfully updated." : response.Message);
        }

        private static void ListSubAccounts()
        {
            _api.SubAccounts.List().Print();
        }

        private static void CustomerRiskAction()
        {
            _api.Customers.BlackList("CUS_bq58eabsts5xvhc").Print();
            _api.Customers.WhiteList("CUS_bq58eabsts5xvhc").Print();
        }

        private static void CustomerUpdate() =>
            _api.Customers.Update("CUS_bq58eabsts5xvhc", "BILL", "Gate Williams III", "08068287222").Print();

        private static void CustomerFetch() =>
            _api.Customers.Fetch("CUS_kwsmfqylmt5lrb8").Print();


        private static void CustomersList() =>
            _api.Customers.List().Print();


        private static void TransactionExport_Setup() =>
            _api.Transactions.Export().Print();


        private static void TransactionTotals_Setup()
        {
            var response = _api.Transactions.Totals();
            Console.WriteLine(
                JsonConvert.SerializeObject(response, Formatting.Indented, PayStackApi.SerializerSettings)
            );
        }

        private static void TransactionTimeline_Setup() =>
            _api.Transactions.Timeline("540314").Print();

        private static void TransactionFetch_Setup() =>
            _api.Transactions.Fetch("540314").Print();


        private static void TransactionList_Setup() =>
            _api.Transactions.List().Print();


        private static void InitializeRequest_Setup()
        {
            var request = new TransactionInitializeRequest
            {
                AmountInKobo = "900000",
                Email = "adebisi-fa@live.com",
                Reference = Guid.NewGuid().ToString(), // or your custom reference
            };

            // Add customer fields
            request.CustomFields.Add(CustomField.From("Name", "name", "ADEBISI Foluso A."));

            // Add other metadata
            request.MetadataObject["DataKey"] = "Containerization (Docker) is super Awesome!";

            // Show what the request JSON looks like
            Console.WriteLine("Request");
            request.Print();
            Console.WriteLine();

            // Initialize api with secret from the <appSettings /> of application configuration file (app.config or web.config)
            var response = _api.Transactions.Initialize(request);

            if (!response.Status) // Initialization failed
            {
                // Display response message and quit!
                var message = response.Message;
                return;
            }
            Console.WriteLine("Response");
            response.Print();
        }
    }

    public static class Extensions
    {
        public static void Print(this object request)
        {
            (request as IPreparable)?.Prepare();

            Console.WriteLine(
                JsonConvert.SerializeObject(request, Formatting.Indented, PayStackApi.SerializerSettings)
            );
        }
    }
}