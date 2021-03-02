﻿using Pds.Contracts.Data.Common.CustomExceptionHandlers;
using Pds.Contracts.Data.Common.Enums;
using Pds.Contracts.Data.Repository.DataModels;
using Pds.Contracts.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Pds.Contracts.Data.Services.Implementations
{
    /// <summary>
    /// Contract validation service.
    /// </summary>
    /// <seealso cref="Pds.Contracts.Data.Services.Interfaces.IContractValidationService" />
    public class ContractValidationService : IContractValidationService
    {
        /// <inheritdoc/>
        public void Validate(Contract contract, Models.ContractRequest request)
        {
            //Validate contract can be found.
            _ = contract ?? throw new ContractNotFoundException(request.ContractNumber, request.ContractVersion, request.Id);

            //Ensure contract id matches the contract number version combination.
            if (request.ContractNumber != contract.ContractNumber || request.ContractVersion != contract.ContractVersion || request.Id != contract.Id)
            {
                throw new InvalidContractRequestException(request.ContractNumber, request.ContractVersion, request.Id);
            }
       }

        /// <inheritdoc/>
        public void Validate(Contract contract, Models.ContractRequest request, Expression<Func<Contract, bool>> validatePredicate)
        {
            Validate(contract, request);

            var validationPredicate = validatePredicate.Compile();
            if (!validationPredicate(contract))
            {
                throw new ContractExpectationFailedException(request.ContractNumber, request.ContractVersion, request.Id, validatePredicate.Body.ToString());
            }
        }

        /// <inheritdoc/>
        public void ValidateStatusChange(Contract contract, ContractStatus newStatus, bool isManualApproval = false)
        {
            var currentStatus = (ContractStatus)contract.Status;
            var message = "Invalid status change detected.";
            var allowedCurrentStatuses = new List<ContractStatus>();

            switch (newStatus)
            {
                case ContractStatus.Approved:
                    allowedCurrentStatuses.Add(isManualApproval ? ContractStatus.PublishedToProvider : ContractStatus.ApprovedWaitingConfirmation);
                    break;

                case ContractStatus.PublishedToProvider:
                case ContractStatus.Replaced:
                case ContractStatus.WithdrawnByProvider:
                case ContractStatus.WithdrawnByAgency:
                case ContractStatus.ApprovedWaitingConfirmation:
                    break;

                default:
                    message = $"Contract is in {currentStatus}, status changes are allowed only when contract is in one of {ContractStatus.PublishedToProvider}, {ContractStatus.WithdrawnByAgency}, {ContractStatus.WithdrawnByProvider}, {ContractStatus.Approved}, {ContractStatus.ApprovedWaitingConfirmation} or {ContractStatus.Replaced} statuses.";
                    break;
            }

            if (!allowedCurrentStatuses.Contains(currentStatus))
            {
                throw new ContractStatusException(message)
                {
                    CurrentStatus = currentStatus,
                    NewStatus = newStatus,
                    AllowedStatuses = allowedCurrentStatuses
                };
            }
        }
    }
}