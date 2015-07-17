

using System;
namespace ME3Data.DataTypes
{
    [FlagsAttribute]
    public enum ObjectFlags : ulong
    {
        None                    = 0,
        Transactional           = 0x0000000100000000U,
        Unreachable             = 0x0000000200000000U,
        Public                  = 0x0000000400000000U,
        TagImp                  = 0x0000000800000000U,
        TagExp                  = 0x0000001000000000U,
        Obsolete                = 0x0000002000000000U,
        TagGarbage              = 0x0000004000000000U,
        Final                   = 0x0000008000000000U,
        PerObjectLocalized      = 0x0000010000000000U,
        NeedLoad                = 0x0000020000000000U,
        HighlightedName         = 0x0000040000000000U,
        EliminateObject         = 0x0000040000000000U,
        InSingularFunc          = 0x0000080000000000U,
        RemappedName            = 0x0000080000000000U,
        Suppress                = 0x0000100000000000U,
        StateChanged            = 0x0000100000000000U, 
        InEndState              = 0x0000200000000000U,
        Transient               = 0x0000400000000000U,
        Preloading              = 0x0000800000000000U,
        LoadForClient           = 0x0001000000000000U,	
        LoadForServer           = 0x0002000000000000U,	
        LoadForEdit             = 0x0004000000000000U,
        Standalone              = 0x0008000000000000U, 
        NotForClient            = 0x0010000000000000U,
        NotForServer            = 0x0020000000000000U,
        NotForEdit              = 0x0040000000000000U,
        Destroyed               = 0x0080000000000000U,
        NeedPostLoad            = 0x0100000000000000U, 
        HasStack                = 0x0200000000000000U,
        Native                  = 0x0400000000000000U,
        Marked                  = 0x0800000000000000U,
        ErrorShutdown           = 0x1000000000000000U,
        DebugPostLoad           = 0x2000000000000000U,
        DebugSerialize          = 0x4000000000000000U,
        DebugDestroy            = 0x8000000000000000U,
    }

    [FlagsAttribute]
    public enum FunctionFlags : uint
    {
        None                    = 0,
        Final                   = 0x00000001U,
        Defined                 = 0x00000002U, 
        Iterator                = 0x00000004U, 
        Latent                  = 0x00000008U,
        PreOperator             = 0x00000010U,
        Singular                = 0x00000020U,
        Net                     = 0x00000040U,
        NetReliable             = 0x00000080U,
        Simulated               = 0x00000100U,
        Exec                    = 0x00000200U,
        Native                  = 0x00000400U,
        Event                   = 0x00000800U, 
        Operator                = 0x00001000U,
        Static                  = 0x00002000U, 
        OptionalParameters      = 0x00004000U,  // TODO: verify if NoExport still exists?
        Const                   = 0x00008000U,
        Invariant               = 0x00010000U,  // TODO: verify if this still exists
        Public                  = 0x00020000U,
        Private                 = 0x00040000U,
        Protected               = 0x00080000U, 
        Delegate                = 0x00100000U,
        NetServer               = 0x00200000U,
        HasOutParms             = 0x00400000U,
        HasDefaults             = 0x00800000U, 
        NetClient               = 0x01000000U,
        FuncInherit             = 0x02000000U,
        FuncOverrideMatch       = 0x04000000U
    }

    [FlagsAttribute]
    public enum PackageFlags : uint
    {
        None                    = 0,
        AllowDownload           = 0x00000001U,
        ClientOptional          = 0x00000002U,
        ServerSideOnly          = 0x00000004U,
        Cooked                  = 0x00000008U,
        Unsecure                = 0x00000010U,
        Encrypted               = 0x00000020U,
        Need                    = 0x00008000U,
        Map                     = 0x00020000U,
        Script                  = 0x00200000U,
        Debug                   = 0x00400000U,
        Imports                 = 0x00800000U,
        Compressed              = 0x02000000U,
        FullyCompressed         = 0x04000000U,
        NoExportsData           = 0x20000000U,
        Stripped                = 0x40000000U,
        Protected               = 0x80000000U,
    }

    [FlagsAttribute]
    public enum FileCompressionFlags : uint
    {
        ZLIB                    = 0x00000001U,
        ZLO                     = 0x00000002U,
        ZLX                     = 0x00000004U,
    }

    [FlagsAttribute]
    public enum PropertyFlags : ulong 
    {
        None                    = 0,
        Editable                = 0x0000000000000001U,
        Const                   = 0x0000000000000002U,
        Input                   = 0x0000000000000004U,
        ExportObject            = 0x0000000000000008U,
        OptionalParm            = 0x0000000000000010U,
        Net                     = 0x0000000000000020U,
        EditFixedSize           = 0x0000000000000040U, // also EditConstArray
        Parm                    = 0x0000000000000080U,
        OutParm                 = 0x0000000000000100U,
        SkipParm                = 0x0000000000000200U,
        ReturnParm              = 0x0000000000000400U,
        CoerceParm              = 0x0000000000000800U,
        Native                  = 0x0000000000001000U,
        Transient               = 0x0000000000002000U,
        Config                  = 0x0000000000004000U,
        Localized               = 0x0000000000008000U,
        Travel                  = 0x0000000000010000U,
        EditConst               = 0x0000000000020000U,
        GlobalConfig            = 0x0000000000040000U,
        Component               = 0x0000000000080000U,
        Init                    = 0x0000000000100000U,
        DuplicateTransient      = 0x0000000000200000U,
        NeedCtorLink            = 0x0000000000400000U,
        NoExport                = 0x0000000000800000U,
        NoImport                = 0x0000000001000000U,
        NoClear                 = 0x0000000002000000U,
        EditInline              = 0x0000000004000000U,
        EdFindable              = 0x0000000008000000U,
        EditInlineUse           = 0x0000000010000000U,
        Deprecated              = 0x0000000020000000U,
        DataBinding             = 0x0000000040000000U, // also EditInlineNotify
        SerializeText           = 0x0000000080000000U,
        RepNotify               = 0x0000000100000000U,
        Interp                  = 0x0000000200000000U,
        NonTransactional        = 0x0000000400000000U,
        EditorOnly              = 0x0000000800000000U,
        NotForConsole           = 0x0000001000000000U,
        RepRetry                = 0x0000002000000000U,
        PrivateWrite            = 0x0000004000000000U,
        ProtectedWrite          = 0x0000008000000000U,
        Archetype               = 0x0000010000000000U,
        EditHide                = 0x0000020000000000U,
        EditTextBox             = 0x0000040000000000U,
        CrossLevelPassive       = 0x0000100000000000U,
        CrossLevelActive        = 0x0000200000000000U,
    }

    [FlagsAttribute]
    public enum StateFlags : uint
    {
        None                    = 0,
        Editable                = 0x00000001U,
        Auto                    = 0x00000002U,
        Simulated               = 0x00000004U,
    }

    [FlagsAttribute]
    public enum ClassFlags : uint
    {
        None = 0,
        Abstract                = 0x00000001U,
        Compiled                = 0x00000002U,
        Config                  = 0x00000004U,
        Transient               = 0x00000008U,
        Parsed                  = 0x00000010U,
        Localized               = 0x00000020U,
        SafeReplace             = 0x00000040U,
        NoExport                = 0x00000100U,
        Placeable               = 0x00000200U,
        PerObjectConfig         = 0x00000400U,
        NativeReplication       = 0x00000800U,
        EditInlineNew           = 0x00001000U,
        CollapseCategories      = 0x00002000U,
        ExportStructs           = 0x00004000U, // Possibly removed
        HasComponents           = 0x00400000U,
        Hidden                  = 0x00800000U,
        Deprecated              = 0x01000000U,
        HideDropDown2           = 0x02000000U,
        Exported                = 0x04000000U,
        NativeOnly              = 0x20000000U,
    }

    [FlagsAttribute]
    public enum ExportFlags : uint
    {
        ForcedExport = 0x00000001U,
    }
}