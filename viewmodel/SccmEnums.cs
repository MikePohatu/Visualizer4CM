namespace viewmodel
{
    public enum TaskSequenceType { Generic = 1, OSD = 2 }
    public enum SccmItemType {
        Application = 1,
        Package = 2,
        MobileProgram=3,
        Script=4,
        SoftwareUpdateGroup = 5,
        ConfigurationBaseline = 6,
        TaskSequence = 7,
        ContentDistribution=8,
        DistributionPointGroup=9,
        DistributionPointHealth=10,
        ConfigurationPolicy=11,
        SoftwareUpdate = 37,
        Device,
        User,
        Collection,
        SMS_DeploymentInfo,
        SMS_DeploymentSummary
    }
}
