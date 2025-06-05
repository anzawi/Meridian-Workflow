namespace LeaveRequestSample.Constants;

public static class LeaveRequestStates
{
    public const string DirectManagerReview = "UnderDirectManagerReview";
    public const string SectionHeadReview = "UnderSectionHeadReview";
    public const string HrReview = "UnderHRReview";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
}

public static class LeaveRequestActions
{
    public const string Approve = "Approve";
    public const string Reject = "Reject";
}