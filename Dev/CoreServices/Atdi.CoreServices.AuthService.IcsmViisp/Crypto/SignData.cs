using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XmlConfiguration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.Pkcs;
using System.Net;
using System.IO;
using System.Threading;
using System.Web;
using System.Security.Cryptography;
using System.Xml.XPath;


namespace Atdi.CoreServices.AuthService.IcsmViisp
{

    public static class SignData
    {
        /// <summary>
        /// Инициализация процесса аутентификации
        /// </summary>
        /// <param name="privateKey">Secret Key file name</param>
        /// <param name="publicKey">Public Key file name</param>
        /// <param name="uri">#uniqueNodeId</param>
        /// <param name="pid">PID</param>
        /// <param name="postbackUrl">Postback Url</param>
        /// <param name="correlationData">Corellation Id</param>
        /// <returns></returns>
        public static XmlDocument InitAuthentication(string privateKey, string publicKey, String uri, string pid, string postbackUrl, string correlationData)
        {
            var certificate = new X509Certificate2(File.ReadAllBytes(publicKey));
            var keyBuffer = Helpers.GetBytesFromPEM(System.IO.File.ReadAllText(privateKey), PemStringType.RsaPrivateKey);
            certificate.PrivateKey = Crypto.DecodeRsaPrivateKey(keyBuffer);

            var stream = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Encoding = Encoding.Unicode,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = true,
                NewLineOnAttributes = false
            }))
            {
                writer.WriteStartElement("authentication", "authenticationRequest", "http://www.epaslaugos.lt/services/authentication");
                writer.WriteAttributeString("xmlns", "authentication", null, "http://www.epaslaugos.lt/services/authentication");
                writer.WriteAttributeString("xmlns", "dsig", null, "http://www.w3.org/2000/09/xmldsig#");
                writer.WriteAttributeString("xmlns", "ns3", null, "http://www.w3.org/2001/10/xml-exc-c14n#");
                writer.WriteAttributeString("id", null, "uniqueNodeId");


                writer.WriteElementString("pid", "http://www.epaslaugos.lt/services/authentication", pid);
                writer.WriteElementString("authenticationProvider", "http://www.epaslaugos.lt/services/authentication", "auth.lt.identity.card");
                writer.WriteElementString("authenticationProvider", "http://www.epaslaugos.lt/services/authentication", "auth.lt.bank");
                writer.WriteElementString("authenticationProvider", "http://www.epaslaugos.lt/services/authentication", "auth.signatureProvider");
                writer.WriteElementString("authenticationProvider", "http://www.epaslaugos.lt/services/authentication", "auth.login.pass");
                writer.WriteElementString("authenticationProvider", "http://www.epaslaugos.lt/services/authentication", "auth.lt.government.employee.card");
                writer.WriteElementString("authenticationProvider", "http://www.epaslaugos.lt/services/authentication", "auth.stork");
                writer.WriteElementString("authenticationProvider", "http://www.epaslaugos.lt/services/authentication", "auth.tsl.identity.card");
                writer.WriteElementString("authenticationAttribute", "http://www.epaslaugos.lt/services/authentication", "lt-personal-code");
                writer.WriteElementString("authenticationAttribute", "http://www.epaslaugos.lt/services/authentication", "lt-company-code");
                writer.WriteElementString("userInformation", "http://www.epaslaugos.lt/services/authentication", "id");
                writer.WriteElementString("userInformation", "http://www.epaslaugos.lt/services/authentication", "firstName");
                writer.WriteElementString("userInformation", "http://www.epaslaugos.lt/services/authentication", "lastName");
                writer.WriteElementString("userInformation", "http://www.epaslaugos.lt/services/authentication", "address");
                writer.WriteElementString("userInformation", "http://www.epaslaugos.lt/services/authentication", "email");
                writer.WriteElementString("userInformation", "http://www.epaslaugos.lt/services/authentication", "phoneNumber");
                writer.WriteElementString("userInformation", "http://www.epaslaugos.lt/services/authentication", "birthday");
                writer.WriteElementString("userInformation", "http://www.epaslaugos.lt/services/authentication", "companyName");
                writer.WriteElementString("postbackUrl", "http://www.epaslaugos.lt/services/authentication", postbackUrl);
                writer.WriteElementString("customData", "http://www.epaslaugos.lt/services/authentication", correlationData);

                writer.WriteEndElement();
                writer.Flush();
            }

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);
            XmlDocument xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.Load(stream);

            SignedXml signedXml = new SignedXml(xmlDocument) { SigningKey = certificate.PrivateKey };
            Reference reference = new Reference { Uri = uri };
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            XmlDsigEnvelopedSignatureTransform transform1 = new XmlDsigEnvelopedSignatureTransform();
            transform1.Algorithm = SignedXml.XmlDsigEnvelopedSignatureTransformUrl;
            reference.AddTransform(transform1);

            XmlDsigEnvelopedSignatureTransform transform2 = new XmlDsigEnvelopedSignatureTransform();
            transform2.Algorithm = SignedXml.XmlDsigExcC14NTransformUrl;

            XmlDsigExcC14NTransform c14n = new XmlDsigExcC14NTransform();
            c14n.InclusiveNamespacesPrefixList = "authentication";

            reference.AddTransform(c14n);
            reference.DigestMethod = SignedXml.XmlDsigSHA1Url;
            signedXml.AddReference(reference);

            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new RSAKeyValue((RSA)certificate.PrivateKey));
            signedXml.KeyInfo = keyInfo;

            XmlDsigExcC14NTransform canMethod = (XmlDsigExcC14NTransform)signedXml.SignedInfo.CanonicalizationMethodObject;
            canMethod.InclusiveNamespacesPrefixList = "authentication";

            signedXml.ComputeSignature();
            XmlElement xmldsigXmlElement = signedXml.GetXml();
            var nodeNew = xmlDocument.ImportNode(xmldsigXmlElement, true);

            var childnodes = xmlDocument.GetElementsByTagName("authentication:authenticationRequest");
            if (childnodes.Count > 0)
            {
                foreach (XmlNode n in childnodes)
                {
                    n.AppendChild(nodeNew);
                    break;
                }
            }
            stream.Close();


            var streamAllDoc = new MemoryStream();

            using (XmlWriter writerXmlAllDoc = XmlWriter.Create(streamAllDoc, new XmlWriterSettings
            {
                Encoding = Encoding.Unicode,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = true,
                NewLineOnAttributes = false
            }))
            {
                writerXmlAllDoc.WriteStartElement("soapenv", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
                writerXmlAllDoc.WriteAttributeString("xmlns", "soapenv", null, "http://schemas.xmlsoap.org/soap/envelope/");
                writerXmlAllDoc.WriteAttributeString("xmlns", "aut", null, "http://www.epaslaugos.lt/services/authentication");
                writerXmlAllDoc.WriteAttributeString("xmlns", "xd", null, "http://www.w3.org/2000/09/xmldsig#");
                writerXmlAllDoc.WriteStartElement("soapenv", "Header", "http://schemas.xmlsoap.org/soap/envelope/");
                writerXmlAllDoc.WriteEndElement();
                writerXmlAllDoc.WriteStartElement("soapenv", "Body", "http://schemas.xmlsoap.org/soap/envelope/");

                writerXmlAllDoc.WriteRaw(xmlDocument.InnerXml);

                writerXmlAllDoc.WriteEndElement();
                writerXmlAllDoc.WriteEndElement();
                writerXmlAllDoc.Flush();
            }

            streamAllDoc.Position = 0;
            streamAllDoc.Seek(0, SeekOrigin.Begin);
            XmlDocument xmlAllDocument = new XmlDocument { PreserveWhitespace = true };
            xmlAllDocument.Load(streamAllDoc);

            streamAllDoc.Close();

            return xmlAllDocument;
        }



        /// <summary>
        /// Формирование авторизационного запроса по заданному адресу, известному PID и номеру ticket 
        /// </summary>
        /// <param name="privateKey">Secret Key file name</param>
        /// <param name="publicKey">Public Key file name</param>
        /// <param name="uri">#uniqueNodeId</param>
        /// <param name="pid">PID</param>
        /// <param name="ticket">Tiket id from 'GetResponseinitAuthentication'</param>
        /// <returns></returns>
        public static XmlDocument AuthenticationData(string privateKey, string publicKey, String uri, string pid, string ticket)
        {
            var certificate = new X509Certificate2(File.ReadAllBytes(publicKey));
            var keyBuffer = Helpers.GetBytesFromPEM(System.IO.File.ReadAllText(privateKey), PemStringType.RsaPrivateKey);
            certificate.PrivateKey = Crypto.DecodeRsaPrivateKey(keyBuffer);

            var stream = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Encoding = Encoding.Unicode,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = true,
                NewLineOnAttributes = false
            }))
            {
                writer.WriteStartElement("authentication", "authenticationDataRequest", "http://www.epaslaugos.lt/services/authentication");
                writer.WriteAttributeString("xmlns", "authentication", null, "http://www.epaslaugos.lt/services/authentication");
                writer.WriteAttributeString("xmlns", "dsig", null, "http://www.w3.org/2000/09/xmldsig#");
                writer.WriteAttributeString("xmlns", "ns3", null, "http://www.w3.org/2001/10/xml-exc-c14n#");
                writer.WriteAttributeString("id", null, "uniqueNodeId");
                writer.WriteElementString("pid", "http://www.epaslaugos.lt/services/authentication", pid);
                writer.WriteElementString("ticket", "http://www.epaslaugos.lt/services/authentication", ticket);
                writer.WriteElementString("includeSourceData", "http://www.epaslaugos.lt/services/authentication", "true");
                writer.WriteEndElement();
                writer.Flush();
            }

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);
            XmlDocument xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.Load(stream);

            SignedXml signedXml = new SignedXml(xmlDocument) { SigningKey = certificate.PrivateKey };
            Reference reference = new Reference { Uri = uri };
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            XmlDsigEnvelopedSignatureTransform transform1 = new XmlDsigEnvelopedSignatureTransform();
            transform1.Algorithm = SignedXml.XmlDsigEnvelopedSignatureTransformUrl;
            reference.AddTransform(transform1);

            XmlDsigEnvelopedSignatureTransform transform2 = new XmlDsigEnvelopedSignatureTransform();
            transform2.Algorithm = SignedXml.XmlDsigExcC14NTransformUrl;

            XmlDsigExcC14NTransform c14n = new XmlDsigExcC14NTransform();
            c14n.InclusiveNamespacesPrefixList = "authentication";

            reference.AddTransform(c14n);
            reference.DigestMethod = SignedXml.XmlDsigSHA1Url;
            signedXml.AddReference(reference);

            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new RSAKeyValue((RSA)certificate.PrivateKey));
            signedXml.KeyInfo = keyInfo;

            XmlDsigExcC14NTransform canMethod = (XmlDsigExcC14NTransform)signedXml.SignedInfo.CanonicalizationMethodObject;
            canMethod.InclusiveNamespacesPrefixList = "authentication";

            signedXml.ComputeSignature();
            XmlElement xmldsigXmlElement = signedXml.GetXml();
            var nodeNew = xmlDocument.ImportNode(xmldsigXmlElement, true);

            var childnodes = xmlDocument.GetElementsByTagName("authentication:authenticationDataRequest");
            if (childnodes.Count > 0)
            {
                foreach (XmlNode n in childnodes)
                {
                    n.AppendChild(nodeNew);
                    break;
                }
            }
            stream.Close();


            var streamAllDoc = new MemoryStream();

            using (XmlWriter writerXmlAllDoc = XmlWriter.Create(streamAllDoc, new XmlWriterSettings
            {
                Encoding = Encoding.Unicode,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = true,
                NewLineOnAttributes = false
            }))
            {
                writerXmlAllDoc.WriteStartElement("soapenv", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
                writerXmlAllDoc.WriteAttributeString("xmlns", "soapenv", null, "http://schemas.xmlsoap.org/soap/envelope/");
                writerXmlAllDoc.WriteAttributeString("xmlns", "aut", null, "http://www.epaslaugos.lt/services/authentication");
                writerXmlAllDoc.WriteAttributeString("xmlns", "xd", null, "http://www.w3.org/2000/09/xmldsig#");
                writerXmlAllDoc.WriteStartElement("soapenv", "Header", "http://schemas.xmlsoap.org/soap/envelope/");
                writerXmlAllDoc.WriteEndElement();
                writerXmlAllDoc.WriteStartElement("soapenv", "Body", "http://schemas.xmlsoap.org/soap/envelope/");

                writerXmlAllDoc.WriteRaw(xmlDocument.InnerXml);

                writerXmlAllDoc.WriteEndElement();
                writerXmlAllDoc.WriteEndElement();
                writerXmlAllDoc.Flush();
            }

            streamAllDoc.Position = 0;
            streamAllDoc.Seek(0, SeekOrigin.Begin);
            XmlDocument xmlAllDocument = new XmlDocument { PreserveWhitespace = true };
            xmlAllDocument.Load(streamAllDoc);

            streamAllDoc.Close();

            return xmlAllDocument;
        }

        /// <summary>
        /// Получение ответа от сервера аутентификации об успешности/проблемах инициализации процедуры аутентификации
        /// </summary>
        /// <param name="url">Authentication request address</param>
        /// <param name="xmlInitAuthentication">xmlRequest from method InitAuthentication</param>
        /// <param name="Fault string">Fault string</param>
        /// <returns>Если метод выполнился удачно, то получаем ticketId</returns>
        public static string GetResponseinitAuthentication(string url, XmlDocument xmlInitAuthentication, out string faultstring)
        {
            string ticketId = "";
            faultstring = "";
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(xmlInitAuthentication.OuterXml);
                request.ContentType = "text/xml; charset=UTF-8";
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                WebResponse response = request.GetResponse();
                using (Stream respStream = response.GetResponseStream())
                {
                    XmlDocument xmlDocument = new XmlDocument { PreserveWhitespace = true };
                    xmlDocument.Load(respStream);
                    var childnodesFault = xmlDocument.GetElementsByTagName("faultstring");
                    if (childnodesFault.Count > 0)
                    {
                        foreach (XmlNode n in childnodesFault)
                        {
                            faultstring = n.InnerText;
                            break;
                        }
                    }
                    var childnodesTicketId = xmlDocument.GetElementsByTagName("authentication:ticket");
                    if (childnodesTicketId.Count > 0)
                    {
                        foreach (XmlNode n in childnodesTicketId)
                        {
                            ticketId = n.InnerText;
                            break;
                        }
                    }
                }
            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;
                using (Stream respStream = errResp.GetResponseStream())
                {
                    XmlDocument xmlDocument = new XmlDocument { PreserveWhitespace = true };
                    xmlDocument.Load(respStream);
                    var childnodesFault = xmlDocument.GetElementsByTagName("faultstring");
                    if (childnodesFault.Count > 0)
                    {
                        foreach (XmlNode n in childnodesFault)
                        {
                            faultstring = n.InnerText;
                            break;
                        }
                    }
                    var childnodesTicketId = xmlDocument.GetElementsByTagName("authentication:ticket");
                    if (childnodesTicketId.Count > 0)
                    {
                        foreach (XmlNode n in childnodesTicketId)
                        {
                            ticketId = n.InnerText;
                            break;
                        }
                    }
                }
            }
            return ticketId;
        }

        /// <summary>
        /// Получение конечного результата по авторизации (возвращаются данные по юзерам успешно прошедшим авторизацию) 
        /// </summary>
        /// <param name="url">Authentication request address</param>
        /// <param name="xmlInitAuthentication">xmlRequest from method 'AuthenticationData'</param>
        /// <param name="faultstring">fault string</param>
        /// <returns></returns>
        public static ResponseInformationDataResponse GetResponseAuthenticationData(string url, XmlDocument xmlInitAuthentication, out string faultstring)
        {
            faultstring = "";
            var responseInformationDataResponse = new ResponseInformationDataResponse();
            var authenticationAttr = new List<AuthenticationAttribute>();
            var userInfo = new UserInformation();
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(xmlInitAuthentication.OuterXml);
                request.ContentType = "text/xml; charset=UTF-8";
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                var response = request.GetResponse();
                using (var respStream = response.GetResponseStream())
                {

                    var xmlDocument = new XmlDocument { PreserveWhitespace = true };
                    xmlDocument.Load(respStream);

                    responseInformationDataResponse.AuthenticationProvider = GetValueByTagName(xmlDocument, "authentication:authenticationProvider");
                    responseInformationDataResponse.CustomData = GetValueByTagName(xmlDocument, "authentication:customData");

                    var authentication_attribute = GetValueByTagName(xmlDocument, "authentication:authenticationAttribute", "authentication:attribute");
                    var authentication_value = GetValueByTagName(xmlDocument, "authentication:authenticationAttribute", "authentication:value");
                    if ((authentication_attribute != null) && (authentication_value != null))
                    {
                        for (int i = 0; i < authentication_attribute.Count; i++)
                        {
                            var authAttribute = new AuthenticationAttribute();
                            authAttribute.Attribute = authentication_attribute[i];
                            authAttribute.Value = authentication_value[i];
                            authenticationAttr.Add(authAttribute);
                        }
                        responseInformationDataResponse.AuthenticationAttribute = authenticationAttr.ToArray();
                    }


                    var authentication_information = GetValueByTagName(xmlDocument, "authentication:userInformation", "authentication:information");
                    var authentication_stringValue = GetValueByTagName(xmlDocument, "authentication:userInformation", "authentication:value", "authentication:stringValue");
                    if ((authentication_information != null) && (authentication_stringValue != null))
                    {
                        for (int i = 0; i < authentication_information.Count; i++)
                        {
                            if (authentication_information[i] == "id")
                            {
                                userInfo.id = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "firstName")
                            {
                                userInfo.firstName = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "lastName")
                            {
                                userInfo.lastName = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "address")
                            {
                                userInfo.address = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "email")
                            {
                                userInfo.email = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "phoneNumber")
                            {
                                userInfo.phoneNumber = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "birthday")
                            {
                                userInfo.birthday = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "companyName")
                            {
                                userInfo.companyName = authentication_stringValue[i];
                            }
                        }

                        responseInformationDataResponse.UserInformation = userInfo;
                    }
                    faultstring = GetValueByTagName(xmlDocument, "faultstring");

                }
            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;

                using (var respStream = errResp.GetResponseStream())
                {

                    var xmlDocument = new XmlDocument { PreserveWhitespace = true };
                    xmlDocument.Load(respStream);

                    responseInformationDataResponse.AuthenticationProvider = GetValueByTagName(xmlDocument, "authentication:authenticationProvider");
                    responseInformationDataResponse.CustomData = GetValueByTagName(xmlDocument, "authentication:customData");

                    var authentication_attribute = GetValueByTagName(xmlDocument, "authentication:authenticationAttribute", "authentication:attribute");
                    var authentication_value = GetValueByTagName(xmlDocument, "authentication:authenticationAttribute", "authentication:value");
                    if ((authentication_attribute != null) && (authentication_value != null))
                    {
                        for (int i = 0; i < authentication_attribute.Count; i++)
                        {
                            var authAttribute = new AuthenticationAttribute();
                            authAttribute.Attribute = authentication_attribute[i];
                            authAttribute.Value = authentication_value[i];
                            authenticationAttr.Add(authAttribute);
                        }
                        responseInformationDataResponse.AuthenticationAttribute = authenticationAttr.ToArray();
                    }


                    var authentication_information = GetValueByTagName(xmlDocument, "authentication:userInformation", "authentication:information");
                    var authentication_stringValue = GetValueByTagName(xmlDocument, "authentication:userInformation", "authentication:value", "authentication:stringValue");
                    if ((authentication_information != null) && (authentication_stringValue != null))
                    {
                        for (int i = 0; i < authentication_information.Count; i++)
                        {
                            if (authentication_information[i] == "id")
                            {
                                userInfo.id = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "firstName")
                            {
                                userInfo.firstName = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "lastName")
                            {
                                userInfo.lastName = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "address")
                            {
                                userInfo.address = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "email")
                            {
                                userInfo.email = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "phoneNumber")
                            {
                                userInfo.phoneNumber = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "birthday")
                            {
                                userInfo.birthday = authentication_stringValue[i];
                            }
                            else if (authentication_information[i] == "companyName")
                            {
                                userInfo.companyName = authentication_stringValue[i];
                            }
                        }

                        responseInformationDataResponse.UserInformation = userInfo;
                    }
                    faultstring = GetValueByTagName(xmlDocument, "faultstring");
                }
            }
            return responseInformationDataResponse;
        }

        private static string GetValueByTagName(XmlDocument xmlDocument, string tagName)
        {
            string value = null;
            var childnodes = xmlDocument.GetElementsByTagName(tagName);
            if (childnodes.Count > 0)
            {
                foreach (XmlNode n in childnodes)
                {
                    value = n.InnerText;
                    break;
                }
            }
            return value;
        }

        private static List<string> GetValueByTagName(XmlDocument xmlDocument, string tagName, string additionalText)
        {
            List<string> values = new List<string>();
            var childnodes = xmlDocument.GetElementsByTagName(tagName);
            if (childnodes.Count > 0)
            {
                foreach (XmlNode n in childnodes)
                {
                    var ch = n.ChildNodes;
                    foreach (XmlNode c in ch)
                    {
                        if (c.Name == additionalText)
                        {
                            values.Add(c.InnerText);
                        }
                    }
                }
            }
            return values;
        }

        private static List<string> GetValueByTagName(XmlDocument xmlDocument, string tagName, string additionalTextLvl0, string additionalTextLvl1)
        {
            List<string> values = new List<string>();
            var childnodes = xmlDocument.GetElementsByTagName(tagName);
            if (childnodes.Count > 0)
            {
                foreach (XmlNode n in childnodes)
                {
                    var ch = n.ChildNodes;
                    foreach (XmlNode c in ch)
                    {
                        if (c.Name == additionalTextLvl0)
                        {
                            bool isFind = false;
                            var chLvl1 = c.ChildNodes;
                            foreach (XmlNode cLvl1 in chLvl1)
                            {
                                if (cLvl1.Name == additionalTextLvl1)
                                {
                                    values.Add(cLvl1.InnerText);
                                    isFind = true;
                                }
                            }
                            if (isFind == false)
                            {
                                values.Add("");
                            }
                        }
                    }
                }
            }
            return values;
        }
    }
}
