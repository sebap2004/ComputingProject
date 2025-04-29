using ComputingProject.Client.Services;
using ComputingProject.Hubs;
using ComputingProject.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SignalR_UnitTestingSupportXUnit.Hubs;

namespace UnitTests;

public class ClassroomHubTests : HubUnitTestsBase<IClassroomClient>
{
    /// <summary>
    /// Test to assert a question is sent to all clients.
    /// </summary>
    [Fact]
    public async Task SendQuestion_ShouldAddQuestionToClassroomService()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        // Act
        // Simulate a client calling the SendTeacherQuestion method
        TeacherQuestion question = new TeacherQuestion() { Id = "123", QuestionText = "Test Question", Answers = new List<string>(), Archived = false};

        await classroomHub.SendTeacherQuestion(question);

        mockClassroomStateService.Verify(x => x.AddQuestion(question), Times.Once);;
    }

    /// <summary>
    /// Test to verify that providing an answer to a teacher's question through the hub
    /// adds the answer to the corresponding question in the classroom state service.
    /// </summary>
    [Fact]
    public async Task AnswerTeacherQuestion_ShouldAddAnswerToAnswerList()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        // Act
        // Simulate a client calling the SendTeacherQuestion method

        await classroomHub.AnswerTeacherQuestion("testID", "testQuestionID", "testAnswer");

        mockClassroomStateService.Verify(x => x.AddAnswerToQuestion("testQuestionID", "testAnswer"), Times.Once);;
    }
    
    /// <summary>
    /// Test to verify deleting a teacher question through the hub removes the question from the classroom service.
    /// </summary>
    [Fact]
    public async Task DeleteTeacherQuestion_ShouldRemoveQuestionFromClassroomService()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        TeacherQuestion question = new TeacherQuestion() { Id = "123", QuestionText = "Test Question", Answers = new List<string>(), Archived = false};

        await classroomHub.DeleteTeacherQuestion(question.Id);
        
        mockClassroomStateService.Verify(x => x.DeleteTeacherQuestion(question.Id), Times.Once);
    }
    
    /// <summary>
    /// Test to verify that sending a help request through the hub adds the help request to the list of help requests.
    /// </summary>
    [Fact]
    public async Task SendHelpRequest_ShouldAddHelpRequestToListOfHelpRequests()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        // Act
        await classroomHub.SendHelpRequest("testID");

        mockClassroomStateService.Verify(x => x.AddHelpRequest("testID"), Times.Once);;
    }
    
    /// <summary>
    /// Test to verify that cancelling a help request through the hub removes the help request from the list of help requests.
    /// </summary>
    [Fact]
    public async Task CancellingHelpRequest_ShouldRemoveHelpRequestFromListOfHelpRequests()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        // Act
        await classroomHub.CancelHelpRequest("testID");

        mockClassroomStateService.Verify(x => x.RemoveHelpRequest("testID"), Times.Once);;
    }
    
    /// <summary>
    /// Test to verify that resolving a help request through the hub removes the help request from the list of help requests.
    /// </summary>
    [Fact]
    public async Task ResolvingHelpRequest_ShouldRemoveHelpRequestFromListOfHelpRequests()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        // Act
        await classroomHub.ResolveHelpRequest("testID");

        mockClassroomStateService.Verify(x => x.RemoveHelpRequest("testID"), Times.Once);;
    }
    
    /// <summary>
    /// Test to verify that archiving a teacher question through the hub toggles the archived status of the question.
    /// </summary>
    [Fact]
    public async Task ArchivingTeacherQuestion_ShouldToggleArchivedStatusOfQuestionInClassromService()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        TeacherQuestion question = new TeacherQuestion() { Id = "123", QuestionText = "Test Question", Answers = new List<string>(), Archived = false};

        // Act
        await classroomHub.ArchiveTeacherQuestion(question.Id);

        mockClassroomStateService.Verify(x => x.ToggleArchivedQuestion(question.Id), Times.Once);;
    }
    
    /// <summary>
    /// Test to verify setting the classroom state through the hub updates the classroom state in the classroom service.
    /// </summary>
    [Fact]
    public async Task SettingClassroomState_ShouldUpdateClassroomStateInClassroomService()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        // Act
        await classroomHub.SetClassroomState(ClassroomState.Lecture);
        Assert.Equal(ClassroomState.Lecture, mockClassroomStateService.Object.classroomState);
        
        await classroomHub.SetClassroomState(ClassroomState.Workshop);
        Assert.Equal(ClassroomState.Workshop, mockClassroomStateService.Object.classroomState);
    }
    
    
    /// <summary>
    /// Test to verify adding an announcement through the hub adds the announcement to the list of announcements in the classroom service.
    /// </summary>
    [Fact]
    public async Task AddAnnouncement_ShouldAddAnnouncementToClassroomServiceList()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        TeacherAnnouncement announcement = new TeacherAnnouncement() { Id = "123", AnnouncementText = "Test Announcement", Archived = false};
        
        await classroomHub.AddAnnouncement(announcement);
        
        mockClassroomStateService.Verify(x => x.AddTeacherAnnouncement(announcement), Times.Once);
    }
    
    /// <summary>
    /// Test to verify removing an announcement through the hub removes the announcement from the list of announcements in the classroom service.
    /// </summary>
    [Fact]
    public async Task RemoveAnnouncement_ShouldRemoveAnnouncementFromClassroomServiceList()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        TeacherAnnouncement announcement = new TeacherAnnouncement() { Id = "123", AnnouncementText = "Test Announcement", Archived = false};
        
        await classroomHub.RemoveAnnouncement(announcement.Id);
        
        mockClassroomStateService.Verify(x => x.RemoveTeacherAnnouncement(announcement.Id), Times.Once);
    }
    
    /// <summary>
    /// Test to verify toggling the visibility of an announcement through the hub toggles the visibility of the announcement in the classroom service.
    /// </summary>
    [Fact]
    public async Task ToggleHideAnnouncement_ShouldToggleVisibilityOfAnnouncementOnClassroomService()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        AssignToHubRequiredProperties(classroomHub);
        
        TeacherAnnouncement announcement = new TeacherAnnouncement() { Id = "123", AnnouncementText = "Test Announcement", Archived = false};

        await classroomHub.ToggleHideAnnouncement(announcement.Id);
        
        mockClassroomStateService.Verify(x => x.ToggleHideTeacherAnnouncement(announcement.Id), Times.Once);;
    }
    
    /// <summary>
    /// Test to verify setting the current task through the hub sets the current task on the classroom service.
    /// </summary>
    [Fact]
    public async Task SetCurrentTask_ShouldSetCurrentTaskOnClassroomService()
    {
        // Arrange
        var mockClassroomStateService = new Mock<ClassroomStateService>();
        var classroomHub = new Classroom(mockClassroomStateService.Object);
        mockClassroomStateService.Object.CurrentTask = "initialised";
        AssignToHubRequiredProperties(classroomHub);
        
        await classroomHub.SetCurrentTask("testTask");
        mockClassroomStateService.Verify(x => x.SetCurrentTask("testTask"), Times.Once);
    }

}
