window.onload = function () {
    var iframe = window.frameElement;
    if (iframe) {
        var dialog = iframe.closest('.ui-dialog');
        if (dialog) {
            var titleBar = dialog.querySelector('.ui-dialog-titlebar');
            if (titleBar) {
                titleBar.remove();
            }
        }
    }
};