﻿using System;
using System.Runtime.Serialization;

namespace Pds.Contracts.Data.Common.CustomExceptionHandlers
{
    /// <summary>
    /// Contract not found exception.
    /// </summary>
    [Serializable]
    public class ContractNotFoundException : Exception
    {
        private const string NotPassedIn = "Not passed";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public ContractNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractNotFoundException"/> class.
        /// Should be used when a contract cannot be found based on the input.
        /// </summary>
        /// <param name="contractNumber">Contract number.</param>
        /// <param name="versionNumber">Contract version number.</param>
        /// <param name="contractId">Contract internal identifier.</param>
        public ContractNotFoundException(string contractNumber, int? versionNumber, int? contractId = null)
            : base($"A contract with contract number:{contractNumber ?? NotPassedIn}, version: {versionNumber?.ToString() ?? NotPassedIn} and contract id: {contractId?.ToString() ?? "Not passed"} cannot be found")
        {
            ContractNumber = contractNumber;
            VersionNumber = versionNumber;
            ContractId = contractId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Serialisation info.</param>
        /// <param name="context">Serialisation context.</param>
        protected ContractNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets ContractNumber.
        /// </summary>
        public string ContractNumber { get; }

        /// <summary>
        /// Gets VersionNumber.
        /// </summary>
        public int? VersionNumber { get; }

        /// <summary>
        /// Gets ContractId.
        /// </summary>
        public int? ContractId { get; }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}