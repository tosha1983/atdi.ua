﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LitvaPortal.AuthenticationManager {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserCredential", Namespace="http://schemas.atdi.com/appserver/services")]
    [System.SerializableAttribute()]
    public partial class UserCredential : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PasswordField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UserNameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Password {
            get {
                return this.PasswordField;
            }
            set {
                if ((object.ReferenceEquals(this.PasswordField, value) != true)) {
                    this.PasswordField = value;
                    this.RaisePropertyChanged("Password");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UserName {
            get {
                return this.UserNameField;
            }
            set {
                if ((object.ReferenceEquals(this.UserNameField, value) != true)) {
                    this.UserNameField = value;
                    this.RaisePropertyChanged("UserName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationResultOfOperationStatePRoijPX3", Namespace="http://schemas.atdi.com/appserver/services")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(LitvaPortal.AuthenticationManager.Result))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(LitvaPortal.AuthenticationManager.ResultOfUserIdentityPRoijPX3))]
    public partial class OperationResultOfOperationStatePRoijPX3 : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FaultCauseField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private LitvaPortal.AuthenticationManager.OperationState StateField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FaultCause {
            get {
                return this.FaultCauseField;
            }
            set {
                if ((object.ReferenceEquals(this.FaultCauseField, value) != true)) {
                    this.FaultCauseField = value;
                    this.RaisePropertyChanged("FaultCause");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LitvaPortal.AuthenticationManager.OperationState State {
            get {
                return this.StateField;
            }
            set {
                if ((this.StateField.Equals(value) != true)) {
                    this.StateField = value;
                    this.RaisePropertyChanged("State");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Result", Namespace="http://schemas.atdi.com/appserver/services")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(LitvaPortal.AuthenticationManager.ResultOfUserIdentityPRoijPX3))]
    public partial class Result : LitvaPortal.AuthenticationManager.OperationResultOfOperationStatePRoijPX3 {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResultOfUserIdentityPRoijPX3", Namespace="http://schemas.atdi.com/appserver/services")]
    [System.SerializableAttribute()]
    public partial class ResultOfUserIdentityPRoijPX3 : LitvaPortal.AuthenticationManager.Result {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private LitvaPortal.AuthenticationManager.UserIdentity DataField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LitvaPortal.AuthenticationManager.UserIdentity Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationState", Namespace="http://schemas.atdi.com/appserver/services")]
    public enum OperationState : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Fault = 1,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserIdentity", Namespace="http://schemas.atdi.com/appserver/services")]
    [System.SerializableAttribute()]
    public partial class UserIdentity : LitvaPortal.AuthenticationManager.UserIdentityOfintUserToken5ExyUEPG {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserIdentityOfintUserToken5ExyUEPG", Namespace="http://schemas.atdi.com/appserver/services")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(LitvaPortal.AuthenticationManager.UserIdentity))]
    public partial class UserIdentityOfintUserToken5ExyUEPG : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private LitvaPortal.AuthenticationManager.UserToken UserTokenField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LitvaPortal.AuthenticationManager.UserToken UserToken {
            get {
                return this.UserTokenField;
            }
            set {
                if ((object.ReferenceEquals(this.UserTokenField, value) != true)) {
                    this.UserTokenField = value;
                    this.RaisePropertyChanged("UserToken");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserToken", Namespace="http://schemas.atdi.com/appserver/services")]
    [System.SerializableAttribute()]
    public partial class UserToken : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] DataField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://schemas.atdi.com/appserver/services/Indentity", ConfigurationName="AuthenticationManager.IAuthenticationManager")]
    public interface IAuthenticationManager {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.atdi.com/appserver/services/Indentity/IAuthenticationManager/Authe" +
            "nticateUser", ReplyAction="http://schemas.atdi.com/appserver/services/Indentity/IAuthenticationManager/Authe" +
            "nticateUserResponse")]
        LitvaPortal.AuthenticationManager.ResultOfUserIdentityPRoijPX3 AuthenticateUser(LitvaPortal.AuthenticationManager.UserCredential credential);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.atdi.com/appserver/services/Indentity/IAuthenticationManager/Authe" +
            "nticateUser", ReplyAction="http://schemas.atdi.com/appserver/services/Indentity/IAuthenticationManager/Authe" +
            "nticateUserResponse")]
        System.Threading.Tasks.Task<LitvaPortal.AuthenticationManager.ResultOfUserIdentityPRoijPX3> AuthenticateUserAsync(LitvaPortal.AuthenticationManager.UserCredential credential);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAuthenticationManagerChannel : LitvaPortal.AuthenticationManager.IAuthenticationManager, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthenticationManagerClient : System.ServiceModel.ClientBase<LitvaPortal.AuthenticationManager.IAuthenticationManager>, LitvaPortal.AuthenticationManager.IAuthenticationManager {
        
        public AuthenticationManagerClient() {
        }
        
        public AuthenticationManagerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AuthenticationManagerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationManagerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationManagerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public LitvaPortal.AuthenticationManager.ResultOfUserIdentityPRoijPX3 AuthenticateUser(LitvaPortal.AuthenticationManager.UserCredential credential) {
            return base.Channel.AuthenticateUser(credential);
        }
        
        public System.Threading.Tasks.Task<LitvaPortal.AuthenticationManager.ResultOfUserIdentityPRoijPX3> AuthenticateUserAsync(LitvaPortal.AuthenticationManager.UserCredential credential) {
            return base.Channel.AuthenticateUserAsync(credential);
        }
    }
}