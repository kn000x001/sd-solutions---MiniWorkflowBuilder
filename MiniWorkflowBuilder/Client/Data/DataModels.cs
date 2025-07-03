public class Workflow
{
    public class WorkflowStep
    {
        public int StepNo { get; set; }
		public int ParentStep { get; set; }		//used for complex nodes like foreach/dowhile/if
        public string Description { get; set; }
        public List<int> Jumps { get; set; }	//connections between steps
        public int ApplicationId { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public List<WorkflowProperty> Properties { get; set; }
        public List<ActivityCondition>? Filters { get; set; }
    }

    public string WorkflowName { get; set; } = "New workflow";
    public string WorkflowDescription { get; set; } = "New workflow description";
    public List<WorkflowStep> Steps { get; set; }
}

public class Application
{
	public int ApplicationId { get; set; }
	public string ApplicationName { get; set; }
	public string ApplicationLogo { get; set; }
	public List<Activity> Activities { get; set; }
	
	public class Activity
	{
		public int ActivityId { get; set; }
		public string ActivityName { get; set; }
		public ActivityType ActivityType { get; set; }
		public ActivityFeatureType ActivityFeatureType { get; set; }
		public List<ActivityProperty> Properties { get; set; }
		public bool IsShowProperties  { get; set; }
		public bool IsShowFilter  { get; set; }
		
		public class ActivityProperty
		{
			public int InputNo { get; set; } = 0;
			public string FieldName { get; set; }
			public string Description { get; set; } = "";
			public string? FieldType { get; set; }	//string,int,datetime...
		}
	}
}

public class  ActivityCondition
{
	public string FieldName { get; set; }
	public ConditionEnum ConditionId { get; set; }
	public string FieldValue { get; set; }
}


public class WorkflowProperty
{
	public string? FieldName { get; set; }
	public string? Description { get; set; }
	public string? Value { get; set; }
}

public enum ActivityType
{
    Trigger = 1,
    Action = 2,
}

public enum ActivityFeatureType
{
    RegularBlock = 0,
    ifblock = 1,
    yesblock = 2,
    noblock = 3,
    Foreach = 4,
    DoWhileBlock = 5,
}

public enum ConditionEnum : int
{
    IsEquals = 1,
    Greater = 2,
    GreaterOrEqual = 3,
    Less = 4,
    LessOrEqual = 5,
    IsNotEqual = 6,
    IsEmpty = 7,
    IsNotEmpty = 8,
    Contains = 9,
    DoesNotContain = 10,
    StartsWith = 11,
    EndsWith = 12,
    DoesNotStartWith = 13,
    DoesNotEndWith = 14,
    In = 15,
    NotIn = 16,
}