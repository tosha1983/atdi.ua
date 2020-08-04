using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Icsm.CoverageEstimation
{
    internal static class Contexts
    {
        public static readonly EventContext ThisComponent = "CoverageEstimation";
        public static readonly EventContext CalcCoverages = "Calculation coverages";
    }

    internal static class Events
    {
        public static readonly EventText StartIterationNumber = "Initial iteration number: {0}";
        public static readonly EventText EndIterationNumber = "Last iteration number: {0}";
        public static readonly EventText ClearFilesFromOutTIFFFilesDirectory = "The operation of clearing files from the 'OutTIFFFilesDirectory' directory completed successfully";
        public static readonly EventText ClearFilesFromICSTelecomProjectDir = "The operation of clearing files from the 'ICSTelecomProjectDir' directory completed successfully";
        public static readonly EventText ClearFilesFromTempTIFFFilesDirectory = "The operation of clearing files from the 'TempTIFFFilesDirectory' directory completed successfully";
        public static readonly EventText OperationSaveImageFileCompleted = "The operation to save the image file '{0}' in the database was completed successfully";
        public static readonly EventText OperationSaveFinalCovarageFileCompleted = "Operation to save final coverage file '{0}' completed successfully";
        public static readonly EventText OperationSaveTempCovarageFileCompleted = "Operation to save temporary coverage file '{0}' completed successfully";
        public static readonly EventText RequestDBStationsCompletedSuccessfully = "The database query to download data on stations was successfully executed";
        public static readonly EventText OperationSaveEWXFileCompleted = "EWX file creation operation '{0}' completed successfully";

    }

    internal static class Categories
    {
        public static readonly EventCategory Disposing = "Disposing";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Executing = "Executing";
        public static readonly EventCategory Running = "Running";
        public static readonly EventCategory Registering = "Registering";
        public static readonly EventCategory Finalizing = "Finalizing";
        public static readonly EventCategory Handling = "Handling";
        public static readonly EventCategory Converting = "Converting";
        public static readonly EventCategory Creating = "Creating";
        public static readonly EventCategory Finishing = "Finishing";
        public static readonly EventCategory Initializing = "Initializing";
    }

    internal static class Exceptions
    {
        public static readonly string FinalCoverageFileTifNotWritenIntoDB = "The final coverage file was not written to the database because it was not found in the path '{0}'";
        public static readonly string FinalCoverageFileTifNotWritenIntoPath = "The final coverage file was not saved in the path '{0}' (for Radiotech = '{1}')";
        public static readonly string FinalCoverageFileTifNotWritenIntoPath2 = "The final coverage file was not saved in the path '{0}' (for FreqConfig = '{1}')";
        public static readonly string OccurredWhilePreparingTemporaryImageTIF = "An error occurred while preparing the temporary image files (TIF) as a result of the operation of combining the contents of the TIF form file and the coverage calculation files (for Radiotech = '{0}')";
        public static readonly string OccurredWhilePreparingTemporaryImageTIF2 = "An error occurred while preparing the temporary image files (TIF) as a result of the operation of combining the contents of the TIF form file and the coverage calculation files (for FreqConfig = '{0}')";
        public static readonly string ResultRequestWebQueryEmptyRecordset = "The result of the WebQuery query is an empty set";
        public static readonly string CodeOperatorAndStatusConfigBlockIsEmpty = "The 'CodeOperatorAndStatusConfig' block in the configuration file is empty";
        public static readonly string CountCodeOperatorAndStatusConfigBlocksLengthZero = "The number of blocks 'CodeOperatorAndStatusConfig' is zero";
        public static readonly string ErrorCallMethodAuthenticateUser = "Error calling method 'AuthenticateUser'";
        public static readonly string ErrorCallMethodGetQueryGroups = "Error calling method 'GetQueryGroups'";
        public static readonly string ErrorCallMethodGetQueryMetadata = "Error calling method 'GetQueryMetadata'";
        public static readonly string BlockCodeOperatorAndStatusConfigIsNull = "The 'CodeOperatorAndStatusConfig' block is empty";
        public static readonly string CountBlock_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigEqualZero = "The number of CodeOperatorAndStatusConfig.StandardConfig.ProvincesConfig blocks is zero";
        public static readonly string Block_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigIsNull = "The CodeOperatorAndStatusConfig.StandardConfig.ProvincesConfig block is empty";
        public static readonly string ICSTelecomProjectFileIsNullOrEmpty = "The ICS Telecom project file name is indented or empty";
        public static readonly string ErrorCopyStationsIntoEwxFile = "An error occurred while copying stations to the EWX file (for Radiotech = '{0}' and Province = '{1}')";
        public static readonly string ErrorCopyStationsIntoEwxFile2 = "An error occurred while copying stations to the EWX file (for FreqConfig = '{0}' and Province = '{1}')";
        public static readonly string Block_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigIsNull = "Block CodeOperatorAndStatusConfig.FreqConfig.ProvincesConfig spaced or empty";
        public static readonly string CountBlock_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigEqualZero = "The number of CodeOperatorAndStatusConfig.FreqConfig.ProvincesConfig blocks is zero";


    }

}
