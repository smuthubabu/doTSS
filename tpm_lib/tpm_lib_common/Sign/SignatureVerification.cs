/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils.Hash;
using Org.BouncyCastle.Crypto;

namespace Iaik.Tc.TPM.Library.Common.Sign
{
    /// <summary>
    /// Static class that verifies a signature generated on the tpm
    /// </summary>
    public static class SignatureVerification
    {
        /// <summary>
        /// Verifies a signature generated on the tpm
        /// </summary>
        /// <param name="keyInfo">The key blob loaded into the tpm</param>
        /// <param name="pubkey">the public key</param>
        /// <param name="data">data to verify for integrity</param>
        /// <param name="signature">signature to verify</param>
        /// <returns></returns>
        public static bool VerifySignature(TPMKey keyInfo, TPMPubkey pubkey, byte[] data, byte[] signature)
        {
            if (keyInfo.AlgorithmParams.SigScheme == TPMSigScheme.TPM_SS_RSASSAPKCS1v15_SHA1)
            {
                byte[] localDataDigest = new HashProvider().Hash(new HashByteDataProvider(data));
                ISigner signatureVerificator = pubkey.CreateSignatureVerificator();
                signatureVerificator.BlockUpdate(data, 0, data.Length);
                return signatureVerificator.VerifySignature(signature);
            }
            else
                throw new NotSupportedException(string.Format("The signature scheme '{0}' is not supported", keyInfo.AlgorithmParams.SigScheme));
        }
    }
}
