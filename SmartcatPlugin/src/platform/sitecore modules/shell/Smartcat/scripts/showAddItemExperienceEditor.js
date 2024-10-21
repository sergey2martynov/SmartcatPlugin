define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
    Sitecore.Commands.ShowAddItemExperienceEditor =
    {
        canExecute: function (context) {
            return true;
        },

        execute: function (context) {
            ExperienceEditor.
                PipelinesUtil.
                generateRequestProcessor("smartcat.showasmarcatmodalpipeline",
                    function (response) {
                        scForm.invoke("smartcat:showadditemexperienceeditor");
                    }).execute(context);
        }
    };
});