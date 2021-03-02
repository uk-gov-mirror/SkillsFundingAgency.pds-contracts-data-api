﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Contracts.Data.Common.CustomExceptionHandlers;
using Pds.Contracts.Data.Common.Enums;
using Pds.Contracts.Data.Services.Implementations;
using Pds.Contracts.Data.Services.Models;
using System;
using DataModels = Pds.Contracts.Data.Repository.DataModels;

namespace Pds.Contracts.Data.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class ContractValidationServiceTests
    {
        private ContractValidationService contractValidationService = new ContractValidationService();

        #region Validate tests

        [TestMethod]
        public void Validate_NoException()
        {
            // Arrange
            var contract = GetContract();
            var contractRequest = GetContractRequest();

            // Act
            Action act = () => contractValidationService.Validate(contract, contractRequest);

            // Assert
            act.Should().NotThrow();
        }

        [TestMethod]
        public void Validate_ContractNotFoundException()
        {
            // Arrange
            DataModels.Contract contract = null;
            var contractRequest = GetContractRequest();

            // Act
            Action act = () => contractValidationService.Validate(contract, contractRequest);

            // Assert
            act.Should().ThrowExactly<ContractNotFoundException>();
        }

        [TestMethod]
        public void Validate_InvalidContractRequestException()
        {
            // Arrange
            var contract = GetContract();
            var contractRequest = GetContractRequest();
            contractRequest.ContractNumber = "xyz";

            // Act
            Action act = () => contractValidationService.Validate(contract, contractRequest);

            // Assert
            act.Should().ThrowExactly<InvalidContractRequestException>();
        }

        [TestMethod]
        public void Validate_InputExpression_NoException()
        {
            // Arrange
            var contract = GetContract();
            contract.ContractContent = GetContractContent();
            var contractRequest = GetContractRequest();

            // Act
            Action act = () => contractValidationService.Validate(contract, contractRequest, c => c.ContractContent != null);

            // Assert
            act.Should().NotThrow();
        }

        [TestMethod]
        public void Validate_InputExpression_ContractExpectationFailedException()
        {
            // Arrange
            var contract = GetContract();
            var contractRequest = GetContractRequest();

            // Act
            Action act = () => contractValidationService.Validate(contract, contractRequest, c => c.ContractContent != null);

            // Assert
            act.Should().ThrowExactly<ContractExpectationFailedException>();
        }

        #endregion Validate tests


        #region ValidateStatusChange tests

        [TestMethod]
        public void ValidateStatusChange_ManualApproval_NoException()
        {
            // Arrange
            ContractStatus newStatus = ContractStatus.Approved;
            bool isManualApproval = true;
            var contract = GetContract();

            // Act
            Action act = () => contractValidationService.ValidateStatusChange(contract, newStatus, isManualApproval);

            // Assert
            act.Should().NotThrow();
        }

        [TestMethod]
        public void ValidateStatusChange_ManualApproval_ContractStatusException()
        {
            // Arrange
            ContractStatus newStatus = ContractStatus.Approved;
            bool isManualApproval = true;
            var contract = GetContract(ContractStatus.Approved);

            // Act
            Action act = () => contractValidationService.ValidateStatusChange(contract, newStatus, isManualApproval);

            // Assert
            act.Should().ThrowExactly<ContractStatusException>();
        }

        [TestMethod]
        public void ValidateStatusChange_NotManualApproval_NoException()
        {
            // Arrange
            ContractStatus newStatus = ContractStatus.Approved;
            bool isManualApproval = false;
            var contract = GetContract(ContractStatus.ApprovedWaitingConfirmation);

            // Act
            Action act = () => contractValidationService.ValidateStatusChange(contract, newStatus, isManualApproval);

            // Assert
            act.Should().NotThrow();
        }

        [TestMethod]
        public void ValidateStatusChange_NotManualApproval_ContractStatusException()
        {
            // Arrange
            ContractStatus newStatus = ContractStatus.Approved;
            bool isManualApproval = false;
            var contract = GetContract();

            // Act
            Action act = () => contractValidationService.ValidateStatusChange(contract, newStatus, isManualApproval);

            // Assert
            act.Should().ThrowExactly<ContractStatusException>();
        }

        #endregion ValidateStatusChange tests


        private DataModels.Contract GetContract(ContractStatus newStatus = ContractStatus.PublishedToProvider)
        {
            return new DataModels.Contract() { Id = 1, ContractNumber = "abc", ContractVersion = 1, Status = (int)newStatus };
        }

        private DataModels.ContractContent GetContractContent()
        {
            return new DataModels.ContractContent() { Id = 1, FileName = "abc" };
        }

        private ContractRequest GetContractRequest()
        {
            return new ContractRequest() { Id = 1, ContractNumber = "abc", ContractVersion = 1 };
        }
    }
}
