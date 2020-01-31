using NearClientUnity.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace NearClientUnity
{
    public class Action
    {
        private readonly dynamic _args;
        private readonly ActionType _type;

        public Action(ActionType type, dynamic args)
        {
            _type = type;
            _args = args;
        }

        public dynamic Args => _args;

        public ActionType Type => _type;

        public static Action AddKey(PublicKey publicKey, AccessKey accessKey)
        {
            dynamic args = new ExpandoObject();
            args.PublicKey = publicKey;
            args.AccessKey = accessKey;
            return new Action(ActionType.AddKey, args);
        }

        public static Action CreateAccount()
        {
            return new Action(ActionType.CreateAccount, null);
        }

        public static Action DeleteAccount(string beneficiaryId)
        {
            dynamic args = new ExpandoObject();
            args.BeneficiaryId = beneficiaryId;
            return new Action(ActionType.DeleteAccount, args);
        }

        public static Action DeleteKey(PublicKey publicKey)
        {
            dynamic args = new ExpandoObject();
            args.PublicKey = publicKey;
            return new Action(ActionType.DeleteKey, args);
        }

        public static Action DeployContract(byte[] code)
        {
            dynamic args = new ExpandoObject();
            args.Code = code;
            return new Action(ActionType.DeployContract, args);
        }

        public static Action FromByteArray(byte[] rawBytes)
        {
            using (var ms = new MemoryStream(rawBytes))
            {
                return FromStream(ms);
            }
        }

        public static Action FromStream(MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public static Action FromStream(ref MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public static Action FunctionCall(string methodName, byte[] methodArgs, ulong? gas, UInt128 deposit)
        {
            dynamic args = new ExpandoObject();
            args.MethodName = methodName;
            args.MethodArgs = methodArgs;
            args.Gas = gas;
            args.Deposit = deposit;
            return new Action(ActionType.FunctionCall, args);
        }

        public static Action Stake(UInt128 stake, PublicKey publicKey)
        {
            dynamic args = new ExpandoObject();
            args.Stake = stake;
            args.PublicKey = publicKey;
            return new Action(ActionType.Stake, args);
        }

        public static Action Transfer(UInt128 deposit)
        {
            dynamic args = new ExpandoObject();
            args.Deposit = deposit;
            return new Action(ActionType.Transfer, args);
        }

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new NearBinaryWriter(ms))
                {
                    writer.Write((byte)_type);

                    switch (_type)
                    {
                        case ActionType.AddKey:
                            {
                                writer.Write(_args.PublicKey.ToByteArray());
                                writer.Write(_args.AccessKey.ToByteArray());
                                return ms.ToArray();
                            }
                        case ActionType.DeleteKey:
                            {
                                writer.Write(_args.PublicKey.ToByteArray());
                                return ms.ToArray();
                            }
                        case ActionType.CreateAccount:
                            {
                                return ms.ToArray();
                            }
                        case ActionType.DeleteAccount:
                            {
                                writer.Write((string)_args.BeneficiaryId);
                                return ms.ToArray();
                            }
                        case ActionType.DeployContract:
                            {
                                writer.Write((uint)_args.Code.Length);
                                writer.Write((byte[])_args.Code);
                                return ms.ToArray();
                            }
                        case ActionType.FunctionCall:
                            {
                                writer.Write((string)_args.MethodName);
                                writer.Write((uint)_args.MethodArgs.Length);
                                writer.Write((byte[])_args.MethodArgs);
                                writer.Write((ulong)_args.Gas);
                                writer.Write((UInt128)_args.Deposit);
                                return ms.ToArray();
                            }
                        case ActionType.Stake:
                            {
                                writer.Write((UInt128)_args.Stake);
                                writer.Write(_args.PublicKey.ToByteArray());
                                return ms.ToArray();
                            }
                        case ActionType.Transfer:
                            {
                                writer.Write((UInt128)_args.Deposit);
                                return ms.ToArray();
                            }
                        default:
                            throw new NotSupportedException("Unsupported action type");
                    }
                }
            }
        }

        private static Action FromRawDataStream(MemoryStream stream)
        {
            using (var reader = new NearBinaryReader(stream, true))
            {
                var actionType = (ActionType)reader.ReadByte();

                switch (actionType)
                {
                    case ActionType.AddKey:
                        {
                            dynamic args = new ExpandoObject();
                            args.PublicKey = PublicKey.FromStream(ref stream);
                            args.AccessKey = AccessKey.FromStream(ref stream);
                            return new Action(ActionType.AddKey, args);
                        }
                    case ActionType.DeleteKey:
                        {
                            dynamic args = new ExpandoObject();
                            args.PublicKey = PublicKey.FromStream(ref stream);
                            return new Action(ActionType.DeleteKey, args);
                        }
                    case ActionType.CreateAccount:
                        {
                            return new Action(ActionType.CreateAccount, null);
                        }
                    case ActionType.DeleteAccount:
                        {
                            dynamic args = new ExpandoObject();
                            args.BeneficiaryId = reader.ReadString();
                            return new Action(ActionType.DeleteAccount, args);
                        }
                    case ActionType.DeployContract:
                        {
                            dynamic args = new ExpandoObject();

                            var byteCount = reader.ReadUInt();

                            var code = new List<byte>();

                            for (var i = 0; i < byteCount; i++)
                            {
                                code.Add(reader.ReadByte());
                            }

                            args.Code = code.ToArray();
                            return new Action(ActionType.DeployContract, args);
                        }
                    case ActionType.FunctionCall:
                        {
                            dynamic args = new ExpandoObject();

                            var methodName = reader.ReadString();

                            var methodArgsCount = reader.ReadUInt();

                            var methodArgs = new List<byte>();

                            for (var i = 0; i < methodArgsCount; i++)
                            {
                                methodArgs.Add(reader.ReadByte());
                            }

                            var gas = reader.ReadULong();

                            var deposit = reader.ReadUInt128();

                            args.MethodName = methodName;
                            args.MethodArgs = methodArgs.ToArray();
                            args.Gas = gas;
                            args.Deposit = deposit;

                            return new Action(ActionType.FunctionCall, args);
                        }
                    case ActionType.Stake:
                        {
                            dynamic args = new ExpandoObject();

                            var stake = reader.ReadUInt128();

                            var publicKey = PublicKey.FromStream(ref stream);

                            args.Stake = stake;
                            args.PublicKey = publicKey;

                            return new Action(ActionType.Stake, args);
                        }
                    case ActionType.Transfer:
                        {
                            dynamic args = new ExpandoObject();

                            var deposit = reader.ReadUInt128();

                            args.Deposit = deposit;

                            return new Action(ActionType.Transfer, args);
                        }
                    default:
                        throw new NotSupportedException("Unsupported action type");
                }
            }
        }
    }
}