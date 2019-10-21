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
        public static readonly EventText StartIterationNumber = "Start iteration number : {0}";
        public static readonly EventText EndIterationNumber = "End iteration number : {0}";
        public static readonly EventText ClearFilesFromOutTIFFFilesDirectory = "The operation to clear files from the 'OutTIFFFilesDirectory' directory is completed successfully";
        public static readonly EventText ClearFilesFromICSTelecomProjectDir = "The operation to clear files from the 'ICSTelecomProjectDir' directory is completed successfully";
        public static readonly EventText ClearFilesFromTempTIFFFilesDirectory = "The operation to clear files from the 'TempTIFFFilesDirectory' directory is completed successfully";
        public static readonly EventText OperationSaveImageFileCompleted = "The operation to saved image file '{0}' to DB is completed successfully";
        public static readonly EventText OperationSaveFinalCovarageFileCompleted = "The operation to saved final covarage file '{0}' is completed successfully";
        public static readonly EventText OperationSaveTempCovarageFileCompleted = "The operation to saved temp covarage file '{0}' is completed successfully";
        public static readonly EventText RequestDBStationsCompletedSuccessfully = "A request to the DB for a sample of stations was completed successfully";
        public static readonly EventText OperationSaveEWXFileCompleted = "The operation created EWX file '{0}' is completed successfully";

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
        public static readonly string FinalCoverageFileTifNotWritenIntoDB = "The final coverage file was not written to the database, because it was not found along the '{0}' path";
        public static readonly string FinalCoverageFileTifNotWritenIntoPath = "The final coverage file was not written to the '{0}' path (for Radiotech = '{1}')";
        public static readonly string FinalCoverageFileTifNotWritenIntoPath2 = "The final coverage file was not written to the '{0}' path (for FreqConfig = '{1}')";
        public static readonly string OccurredWhilePreparingTemporaryImageTIF = "An error occurred while preparing temporary image files(TIF), which are the result of the operation of combining the contents of the blank TIF file and a  coverages files (for Radiotech = '{0}')";
        public static readonly string OccurredWhilePreparingTemporaryImageTIF2 = "An error occurred while preparing temporary image files(TIF), which are the result of the operation of combining the contents of the blank TIF file and a  coverages files (for FreqConfig = '{0}')";
        public static readonly string ResultRequestWebQueryEmptyRecordset = "Result of request to the WebQuery is empty recordset";
        public static readonly string CodeOperatorAndStatusConfigBlockIsEmpty = "Block 'CodeOperatorAndStatusConfig' in config file is empty";
        public static readonly string CountCodeOperatorAndStatusConfigBlocksLengthZero = "Count of blocks 'CodeOperatorAndStatusConfig' equal zero";
        public static readonly string ErrorCallMethodAuthenticateUser = "'AuthenticateUser' method call error ";
        public static readonly string ErrorCallMethodGetQueryGroups = "'GetQueryGroups' method call error ";
        public static readonly string ErrorCallMethodGetQueryMetadata = "'GetQueryMetadata' method call error ";
        public static readonly string TokenStationsCalcCoverageNotFound = "Token for query code 'StationsCalcCoverage' not found";
        public static readonly string TokenResultCalcCoverageNotFound = "Token for query code 'ResultCalcCoverage' not found";
        public static readonly string BlockCodeOperatorAndStatusConfigIsNull = "Block 'CodeOperatorAndStatusConfig' is null";
        public static readonly string CountBlock_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigEqualZero = "Count blocks CodeOperatorAndStatusConfig.StandardConfig.ProvincesConfig equal zero";
        public static readonly string Block_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigIsNull = "Block CodeOperatorAndStatusConfig.StandardConfig.ProvincesConfig is null";
        public static readonly string ICSTelecomProjectFileIsNullOrEmpty = "Project file name of ICS Telecom is null or empty";
        public static readonly string ErrorCopyStationsIntoEwxFile = "An error occurred while copying stations to the EWX file (for Radiotech = '{0}')";
        public static readonly string ErrorCopyStationsIntoEwxFile2 = "An error occurred while copying stations to the EWX file (for FreqConfig = '{0}')";
        public static readonly string Block_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigIsNull = "Block CodeOperatorAndStatusConfig.FreqConfig.ProvincesConfig is null";
        public static readonly string CountBlock_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigEqualZero = "Count blocks CodeOperatorAndStatusConfig.FreqConfig.ProvincesConfig equal zero";


    }

}
