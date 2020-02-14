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
        public static readonly EventText StartIterationNumber = "Початковий номер ітерації: {0}";
        public static readonly EventText EndIterationNumber = "Останній номер ітерації: {0}";
        public static readonly EventText ClearFilesFromOutTIFFFilesDirectory = "Операція очищення файлів з каталогу 'OutTIFFFilesDirectory' успішно завершена";
        public static readonly EventText ClearFilesFromICSTelecomProjectDir = "Операція очищення файлів із каталогу 'ICSTelecomProjectDir' успішно завершена";
        public static readonly EventText ClearFilesFromTempTIFFFilesDirectory = "Операція очищення файлів з каталогу 'TempTIFFFilesDirectory' успішно завершена";
        public static readonly EventText OperationSaveImageFileCompleted = "Операція зі збереження графічного файлу '{0}' в БД успішно завершена";
        public static readonly EventText OperationSaveFinalCovarageFileCompleted = "Операція зі збереження остаточного файлу покриття '{0}' успішно завершена";
        public static readonly EventText OperationSaveTempCovarageFileCompleted = "Операція зі збереження  тимчасового файлу покриття '{0}' успішно завершена";
        public static readonly EventText RequestDBStationsCompletedSuccessfully = "Запит до БД для завантаження даних по станціях було успішно виконано";
        public static readonly EventText OperationSaveEWXFileCompleted = "Операція створення файла EWX '{0}', успішно завершена";

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
        public static readonly string FinalCoverageFileTifNotWritenIntoDB = "Остаточний файл покриття не був записаний у базу даних, оскільки його не було знайдено по шляху '{0}'";
        public static readonly string FinalCoverageFileTifNotWritenIntoPath = "Остаточний файл покриття не було збережено по шляху '{0}' (для Radiotech = '{1}')";
        public static readonly string FinalCoverageFileTifNotWritenIntoPath2 = "Остаточний файл покриття не було збережено  по шляху '{0}' (для FreqConfig = '{1}')";
        public static readonly string OccurredWhilePreparingTemporaryImageTIF = "Під час підготовки тимчасових файлів зображень (TIF) сталася помилка, яка є результатом операції поєднання вмісту  файлу TIF-бланку та розрахункових файлів покриття (для Radiotech = '{0}')";
        public static readonly string OccurredWhilePreparingTemporaryImageTIF2 = "Під час підготовки тимчасових файлів зображень (TIF) сталася помилка, яка є результатом операції поєднання вмісту файлу TIF-бланку та розрахункових файлів покриття (для FreqConfig = '{0}')";
        public static readonly string ResultRequestWebQueryEmptyRecordset = "Результатом запиту до WebQuery є порожній набір ";
        public static readonly string CodeOperatorAndStatusConfigBlockIsEmpty = "Блок 'CodeOperatorAndStatusConfig' у конфігураційному файлі порожній";
        public static readonly string CountCodeOperatorAndStatusConfigBlocksLengthZero = "Кількість блоків 'CodeOperatorAndStatusConfig' дорівнює нулю";
        public static readonly string ErrorCallMethodAuthenticateUser = "Помилка виклику методу 'AuthenticateUser'";
        public static readonly string ErrorCallMethodGetQueryGroups = "Помилка виклику методу 'GetQueryGroups' ";
        public static readonly string ErrorCallMethodGetQueryMetadata = "Помилка виклику методу 'GetQueryMetadata'";
        public static readonly string TokenStationsCalcCoverageNotFound = "Токен для коду запиту 'StationsCalcCoverage' не знайдено";
        public static readonly string TokenResultCalcCoverageNotFound = "Токен для коду запиту 'ResultCalcCoverage' не знайдено";
        public static readonly string BlockCodeOperatorAndStatusConfigIsNull = "Блок 'CodeOperatorAndStatusConfig' є пустим";
        public static readonly string CountBlock_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigEqualZero = "Кількість блоків CodeOperatorAndStatusConfig.StandardConfig.ProvincesConfig дорівнює нулю";
        public static readonly string Block_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigIsNull = "Блок CodeOperatorAndStatusConfig.StandardConfig.ProvincesConfig є пустим";
        public static readonly string ICSTelecomProjectFileIsNullOrEmpty = "Ім'я файлу проекту ICS Telecom відстунє або порожнє";
        public static readonly string ErrorCopyStationsIntoEwxFile = "Під час копіювання станцій у файл EWX сталася помилка (для Radiotech = '{0}' та Province = '{1}')";
        public static readonly string ErrorCopyStationsIntoEwxFile2 = "Під час копіювання станцій у файл EWX сталася помилка(для FreqConfig = '{0}' та Province = '{1}')";
        public static readonly string Block_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigIsNull = "Блок CodeOperatorAndStatusConfig.FreqConfig.ProvincesConfig відстуній або порожній";
        public static readonly string CountBlock_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigEqualZero = "Кількість блоків CodeOperatorAndStatusConfig.FreqConfig.ProvincesConfig дорівнює нулю";


    }

}
