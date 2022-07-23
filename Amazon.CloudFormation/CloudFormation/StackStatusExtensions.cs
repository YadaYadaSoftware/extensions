namespace Amazon.CloudFormation;

public static class StackStatusExtensions
{
    public static bool IsActive(this StackStatus status)
    {
        return status == StackStatus.CREATE_COMPLETE 
               || status == StackStatus.IMPORT_COMPLETE 
               || status == StackStatus.UPDATE_COMPLETE 
               || status == StackStatus.UPDATE_ROLLBACK_COMPLETE ;
    }

    public static bool IsRollingBack(this StackStatus status)
    {
        return status == StackStatus.ROLLBACK_IN_PROGRESS || status == StackStatus.UPDATE_ROLLBACK_IN_PROGRESS || status == StackStatus.UPDATE_ROLLBACK_COMPLETE_CLEANUP_IN_PROGRESS;
    }
    public static bool IsCleaningUp(this StackStatus status)
    {
        return status == StackStatus.UPDATE_COMPLETE_CLEANUP_IN_PROGRESS;
    }

}