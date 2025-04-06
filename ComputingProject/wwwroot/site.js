window.addEventListener('beforeunload', function (e) {
    // Custom message (most modern browsers override this with their own default message)
    const confirmationMessage = 'Are you sure you want to leave? Changes you made may not be saved.';
    // Standard way to trigger the confirmation dialog
    e.preventDefault();
    e.returnValue = confirmationMessage; // For older browsers
    return confirmationMessage;          // For modern browsers
});