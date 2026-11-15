using UnityEngine;

// School: a course costs money and one action now, and raises the salary of every month after it.
public class SchoolBuilding : Building
{
    [SerializeField] private MonthCycle monthCycle;
    [SerializeField] private Wallet wallet;
    [SerializeField] private Career career;
    [SerializeField] private int courseCost = 5000;
    [SerializeField] private int salaryRaise = 700;
    [SerializeField] private int maxCourses = 3;

    private int coursesTaken;

    public override string Title => "School";

    public override string Info => "Courses taken: " + coursesTaken + " of " + maxCourses + ". Salary now: " + career.Salary + ".";

    public override BuildingAction[] GetActions()
    {
        string label = "Take a course: -" + courseCost + " now, +" + salaryRaise + " salary every month (1 action)";
        return new[]
        {
            new BuildingAction(label, TakeCourse),
        };
    }

    private string TakeCourse()
    {
        if (coursesTaken >= maxCourses)
        {
            return "You already took every course.";
        }

        if (!monthCycle.HasActions)
        {
            return NoActionsLeft;
        }

        if (!wallet.TrySpend(courseCost))
        {
            return "Not enough cash for the course.";
        }

        monthCycle.UseAction();
        coursesTaken++;
        career.Raise(salaryRaise);
        return "Course finished! Your salary is now " + career.Salary + " a month.";
    }
}
