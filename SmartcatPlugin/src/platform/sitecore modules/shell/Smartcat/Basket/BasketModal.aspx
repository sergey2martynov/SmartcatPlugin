<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasketModal.aspx.cs" 
Inherits="SmartcatPlugin.sitecore_modules.shell.Smartcat.Basket.BasketModal" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <script src="https://cdn.jsdelivr.net/npm/vue@2"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <script src="https://unpkg.com/element-ui/lib/umd/locale/en.js"></script>
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://unpkg.com/vue2-datepicker/index.css">
    <link href="../common.css" rel="stylesheet" type="text/css"/>
    <script src="https://unpkg.com/vue2-datepicker"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.1/moment.min.js"></script>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans:wght@400;600&display=swap" rel="stylesheet">
</head>
<body>
    <div id="app">
        <el-container>
            <el-aside class="aside-with-divider">
                <el-steps direction="vertical" 
                          :active="currentStep" 
                          class="custom-stepper" 
                          process-status="process" 
                          finish-status="finish">
                    <el-step v-for="(step, index) in steps" :key="index" :title="step.title" />
                </el-steps>
            </el-aside>
        <el-container direction="vertical">
            <div v-if="currentStep === 0" class="content">
                <div class="header-wrapper">
                    <lable class="header">
                        Select pages for translation
                    </lable>
                </div>
                    <div class="tree">
                        <div v-for="node in treeData" :key="node.id">
                            <div class="tree-node">
                                <span class="toggle-icon" @click="toggleNode(node)">
                                    <i :class="node.isExpanded ? 'el-icon-caret-bottom' : 'el-icon-caret-right'"></i>
                                </span>
                                <el-checkbox v-if="node.showCheckBox" 
                                             class="checkbox" 
                                             v-model="node.isChecked" 
                                             @change="handleCheckboxChange(node)">
                                </el-checkbox>
                                <div class="tree-node-content">
                                    <img :src="node.imageUrl" alt="icon">
                                    <span>{{ node.name }}</span>
                                </div>
                            </div>
                            <div class="tree-children" :class="{ active: node.isExpanded }">
                                <tree-node v-if="node.children" 
                                           :nodes="node.children" 
                                           @toggle-node="toggleNode" 
                                           @handle-checkbox-change="handleCheckboxChange"></tree-node>
                            </div>
                        </div>
                    </div>
            </div>
                <div v-if="currentStep === 1" 
                     style="display: grid; 
                            grid-template-rows: auto 1fr;
                            grid-template-columns: 1fr 1fr; 
                            gap: 20px;">
                    <el-container style="grid-column: 1 / span 2; text-align: center;">
                        <el-col>
                            <div class="header-wrapper">
                                <h3 class="header">
                                    Select the target language(s)
                                </h3>
                                <label @click="showHelpModal = true" style="margin-right: 15px; font-size: 12px; cursor: pointer; text-decoration: underline dotted;">
                                    How to add more languages
                                </label>
                                <!-- Модальное окно -->
                                <el-dialog
                                    :visible.sync="showHelpModal"
                                    width="50%"
                                    :show-close="false"
                                    @close="showHelpModal = false">
                                    <div style="padding: 15px; text-align: left;">
                                        <ol style="margin: 0;
                                                   padding-left: 20px;
                                                   font-size: 16px;
                                                   line-height: 1.6;"><li style="margin-bottom: 15px;"> Select the desired page in the navigation <br>tree on the left.</li>
                                            <li style="margin-bottom: 15px;"> In the content area that opens, in the upper right corner, open the language <br>switcher dropdown and click "More <br>languages."</li>
                                            <li>In the window that appears, select the <br>required language and confirm your <br>selection by clicking "OK."</li></ol>
                                    </div>
                                </el-dialog>
                            </div>
                        </el-col>
                    </el-container>
                    <el-col>
                        <label style="padding-top: 15px; font-weight: bold; margin-left: 15px; font-size: 12px">Target language</label>
                        <el-row style="padding-top: 15px; margin-left: 15px;" v-for="targetLanguage in targetLanguages" :key="targetLanguage.code">
                            <el-checkbox
                                v-model="selectedTargetLanguages"
                                :label="targetLanguage.code"
                                @change="handleTargetLanguageChange">
                                {{ targetLanguage.name }}
                            </el-checkbox>
                        </el-row>
                    </el-col>
                    <el-col>
                        <label style="padding-top: 15px; font-weight: bold; font-size: 12px" >Source language</label>
                        <el-row style="padding-top: 15px; font-size: 12px;" v-for="sourceLanguage in sourceLanguages" :key="sourceLanguage.code">
                            <label>{{ sourceLanguage.name }}</label>
                            <!--
                            <el-checkbox
                                :checked="selectedSourceLanguage.includes(sourceLanguage.code)"
                                :label="sourceLanguage.code"
                                @change="handleSourceLanguageChange(sourceLanguage)">
                                {{ sourceLanguage.name }}
                            </el-checkbox>
                            -->
                            
                        </el-row>
                    </el-col>
                </div>
                <el-container v-if="currentStep === 2" class="content" style="font-size: 12px;">
                    <div class="header-wrapper">
                        <h3 class="header">
                            Specify project details
                        </h3>
                    </div>
                    <div style="margin-left: 15px; margin-right: 15px; padding-top: 20px;">
                        <div>
                            <el-label class="project-field-label" for="projectName">
                                Project name <span class="required">*</span>
                            </el-label>
                        </div>
                        <div class="project-field">
                            <el-input 
                                type="text" 
                                id="projectName" 
                                v-model="projectName"
                                size="medium"/>
                        </div>
                    </div>
                    <div style="margin-left: 15px; margin-right: 15px;">
                        <div style="padding-top: 20px;">
                            <el-label class="project-field-label">
                                Deadline
                            </el-label>
                        </div>
                        <div class="project-field datetime-picker-wrapper">
                            <el-date-picker
                                v-model="deadlineDate"
                                type="date"
                                placeholder="Selecet a date"
                                size="medium"
                                style="width: 140px"
                                class="custom-placeholder"
                                format="dd/MM/yyyy"
                                value-format="yyyy-MM-dd">
                            </el-date-picker>
                            <el-select
                                v-model="deadlineTime"
                                placeholder="Select a time"
                                size="medium"
                                style="width: 125px;"
                                class="custom-placeholder">
                                <el-option
                                    v-for="time in timeOptions"
                                    :key="time.value"
                                    :label="time.label"
                                    :value="time.value">
                                </el-option>
                            </el-select>
                            <!-- <el-date-picker
                                v-model="deadline"
                                type="datetime"
                                placeholder="Set a deadline"
                                size="medium"
                                style="width: 100%"
                                default-time="12:00:00"
                            >
                            </el-date-picker> -->
                        </div>
                    </div>
                    <div v-if="isUseTemplates" style="padding-top: 20px; margin-left: 15px; margin-right: 15px;">
                        <div>
                            <el-label class="project-field-label" for="workflowStagesSelect">
                                Templates <span class="required">*</span>
                            </el-label>
                        </div>
                        <div class="project-field">
                            <el-select
                                v-model="selectedTemplateId"
                                size="medium"
                                style="width: 100%"
                                @change="validateLanguages">
                                <el-option
                                    v-for="item in templates"
                                    :key="item.templateId"
                                    :label="item.templateName"
                                    :value="item.templateId">
                                </el-option>
                            </el-select>
                        </div>
                    </div>
                    <div v-if="!isUseTemplates" style="padding-top: 20px; margin-left: 15px; margin-right: 15px;">
                        <div>
                            <el-label class="project-field-label" for="workflowStagesSelect">
                                Workflow Stages <span class="required">*</span>
                            </el-label>
                        </div>
                        <div class="project-field">
                            <el-select
                                v-model="selectedWorkflowStage"
                                size="medium"
                                style="width: 100%">
                                <el-option
                                    v-for="item in workflowStages"
                                    :key="item.id"
                                    :label="item.name"
                                    :value="item.id">
                                </el-option>
                            </el-select>
                        </div>
                    </div>
                    <el-row v-if="!isValidSelectedLanguages" style="padding-top: 10px; padding-bottom: auto; margin-left: 15px; margin-right: 15px;" class="text-wrapper">
                        {{ invalidLanguagesMessage }}
                    </el-row>
                </el-container>
                    <el-container v-if="currentStep === 3" class="content">
                            <div class="header-wrapper">
                                <label class="header">
                                    Confirm the accuracy of the entered data
                                </label>
                            </div>
                        <div style="display: flex; flex-direction: row; gap: 20px">
                            <div style="text-align: left; font-size: 12px; width: 130px">
                                <el-row class="project-info-field-name">
                                    <label style="text-align: left; font-weight: bold;">
                                        Project name
                                    </label>
                                </el-row>
                                <el-row class="project-info-field-name">
                                    <label >
                                        Deadline
                                    </label>
                                </el-row>
                                <el-row class="project-info-field-name">
                                    <label style="text-align: left; font-weight: bold;">
                                        Source language
                                    </label>
                                </el-row>
                                <el-row class="project-info-field-name">
                                    <label style="text-align: left; font-weight: bold;">
                                        Target language
                                    </label>
                                </el-row>
                                <el-row v-if="isUseTemplates" class="project-info-field-name">
                                    <label style="text-align: left; font-weight: bold;">
                                        Template
                                    </label>
                                </el-row>
                                <el-row v-else class="project-info-field-name">
                                    <label style="text-align: left; font-weight: bold;">
                                        Workflow stage
                                    </label>
                                </el-row>
                            </div>
                            <div style="text-align: left; font-size: 12px;">
                                <el-row class="project-info-field-value">
                                    <a>
                                        {{ projectName }}
                                    </a>
                                </el-row>
                                <el-row class="project-info-field-value">
                                    <a>
                                        {{ getFullDeadline() }}
                                    </a>
                                </el-row>
                                <el-row class="project-info-field-value">
                                    <a>
                                        {{ selectedSourceLanguageName }}
                                    </a>
                                </el-row>
                                <el-row class="project-info-field-value">
                                    <a>
                                        {{ selectedTargetLanguageNames.join(', ') }}
                                    </a>
                                </el-row>
                                <el-row v-if="isUseTemplates" class="project-info-field-value">
                                    <a>
                                        {{ getSelectedTemplateName }}
                                    </a>
                                </el-row>
                                <el-row v-else class="project-info-field-value">
                                    <a>
                                        {{ selectedWorkFlowStage.name }}
                                    </a>
                                </el-row>
                            </div>
                        </div>
                </el-container>
            <el-footer class="footer-container" style="height: 65px">
                <div class="left-buttons">
                    <button
                        class="common-button cancel-button"
                        v-if="currentStep === 0"
                        @click="cancel">
                        Cancel
                    </button>
                    <button
                        class="common-button cancel-button"
                        v-else
                        @click="prevStep">
                        Back
                    </button>
                </div>
                <div class="right-buttons">
                    <button
                        v-if="currentStep < 3"
                        :disabled="isDisableNextButton"
                        class="common-button submit-button"
                        @click="nextStep">
                        Next
                    </button>
                    <button
                        v-if="currentStep === 3"
                        class="common-button submit-button"
                        @click="confirmProject"
                        style="width: 140px">
                        Confirm project
                    </button>
                </div>
            </el-footer>
            </el-container>
        </el-container>
    </div>
<script>
    ELEMENT.locale(ELEMENT.lang.en);

    Vue.component('tree-node', {
        props: ['nodes'],
        template: `
    <div>
        <div v-for="node in nodes" :key="node.id">
            <div class="tree-node">
                <span class="toggle-icon" @click="$emit('toggle-node', node)">
                    <i :class="node.isExpanded ? 'el-icon-caret-bottom' : 'el-icon-caret-right'"></i>
                </span>
                <el-checkbox v-if="node.showCheckBox"
                class="checkbox"
                v-model="node.isChecked"
                @change="$emit('handle-checkbox-change', node)">
                </el-checkbox>
                <div class="tree-node-content">
                    <img :src="node.imageUrl" alt="icon">
                    <span>{{ node.name }}</span>
                </div>
            </div>
            <div class="tree-children" :class="{ active: node.isExpanded }">
                <tree-node v-if="node.children"
                    :nodes="node.children"
                    @toggle-node="$emit('toggle-node', $event)"
                    @handle-checkbox-change="$emit('handle-checkbox-change', $event)">
                </tree-node>
            </div>
        </div>
    </div>
`
    });

    new Vue({
        el: '#app',
        data: {
            currentStep: 0,
            steps: [
                { title: 'Page selection' },
                { title: 'Language selection' },
                { title: 'Project details' },
                { title: 'Confirmation' }
            ],
            totalSteps: 4,
            treeData: [],
            checkedNodes: [],
            allNodeIds: [],
            invalidItemNames: "",
            invalidItemCount: 0,
            validItemCount: 0,
            showHelpModal: false,
            projectName: "",
            selectedWorkflowStage: null,
            selectedTemplateId: "",
            deadlineDate: null,
            deadlineTime: null,
            templates: null,
            isUseTemplates: false,
            workflowStages: [
                {
                    name: "Translation",
                    id: "0"
                },
                {
                    name: "AI Translation",
                    id: "1"
                },
                {
                    name: "AI Translation + Translation review",
                    id: "2"
                }
            ],
            defaultTemplate: null,
            sourceLanguages: [],
            targetLanguages: [],
            smartcatLanguageCodes: [],
            selectedSourceLanguage: [],
            selectedTargetLanguages: [],
            selectedSourceLanguageName: "",
            selectedTargetLanguageNames: [],
            isValidSelectedLanguages: true,
            invalidLanguagesMessage: "",
            summaryData: new Map(),
            timeOptions: []
        },
        computed: {
            isDisableNextButton() {
                if (this.currentStep === 1) {
                    console.log(this.selectedTargetLanguages);
                    console.log(this.selectedSourceLanguage);
                }
                
                if ((this.currentStep === 0 && this.checkedNodes.length > 0) ||
                    (this.currentStep === 1 && this.selectedTargetLanguages.length > 0 && this.selectedSourceLanguage.length > 0) ||
                    (this.currentStep === 2 && this.projectName && this.isValidSelectedLanguages &&
                        (this.selectedWorkflowStage || (this.selectedTemplateId && this.isUseTemplates)))) {

                    return false;
                }

                return true;
            },

            getSelectedTemplateName() {
                const template = this.templates.find(item => item.templateId === this.selectedTemplateId);
                return template.templateName;
            }
        },
        watch: {
            deadlineTime(newTime) {
                if (newTime) {
                    this.deadlineTime = moment(newTime, 'HH:mm').format('hh:mm A');
                }
            }
        },
        created() {
            this.getTreeData();
            this.getLanguages();
            this.getTemplates();
            this.generateTimeOptions();
        },
        methods: {
            getFullDeadline() {
                if (this.deadlineDate && this.deadlineTime) {
                    return `${this.deadlineDate} ${this.deadlineTime}`;
                }
                return "No date selected";
            },
            getTreeData() {
                axios.get('/api/additem/get-items-tree')
                    .then(response => {
                        this.treeData = response.data.TreeNodes;
                        this.checkedNodes = response.data.CheckedItems;
                        this.allNodeIds = response.data.ExpandedItems;
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            getLanguages() {
                axios.get('/api/basket/get-translation-languages')
                    .then(response => {
                        this.sourceLanguages = response.data.sourceLanguages;
                        this.selectedSourceLanguage = this.sourceLanguages.map(l => l.code);
                        this.targetLanguages = response.data.targetLanguages;
                        this.smartcatLanguageCodes = response.data.smartcatLanguageCodes;
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            async getTemplates() {
                axios.get('/api/basket/get-templates')
                    .then(response => {
                        console.log("get-templates", response);
                        this.templates = response.data.templates;
                        this.isUseTemplates = response.data.templates.length > 0;
                        if (this.isUseTemplates) {
                            this.selectedTemplateId = this.templates[0].templateId;
                            const defaultTemplate = {
                                sourceLocales: this.sourceLanguages.map(lang => lang.code),
                                targetLocales: this.targetLanguages.map(lang => lang.code),
                                templateId: '2',
                                templateName: this.workflowStages[2].name
                            };

                            this.templates.push(defaultTemplate);
                        } else {

                            this.selectedWorkFlowStage = this.workflowStages[2];
                        }
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            validateLanguages(templateId) {
                if (!this.isUseTemplates) {
                    return;
                }

                const template = this.templates.find(item => item.templateId === templateId);
                let sourceLanguageMessage = "";
                this.isValidSelectedLanguages = true;

                if (!template.sourceLocales.includes(this.selectedSourceLanguage[0])) {
                    this.isValidSelectedLanguages = false;
                    sourceLanguageMessage = this.selectedSourceLanguage.name + " (source language).";
                }

                let targetLanguageMessage = "";

                if (!this.selectedTargetLanguages.every(element => template.targetLocales.includes(element))) {
                    this.isValidSelectedLanguages = false;
                    targetLanguageMessage = this.selectedTargetLanguages.filter(element => !template.sourceLocales.includes(element))
                        + "(target languages). ";
                }

                if (!this.isValidSelectedLanguages) {
                    this.invalidLanguagesMessage = template.templateName +
                        " does not support the selected languages: " +
                        sourceLanguageMessage +
                        targetLanguageMessage +
                        "Please go back and adjust your language selection or select another template.";
                }
            },
            nextStep() {
                
                if (this.currentStep === 1) {
                    this.validateLanguages(this.selectedTemplateId);
                }
                if (this.currentStep === 2) {
                    this.formatDeadline();
                    this.handleSelectedLanguages();
                }
                if (this.currentStep < this.totalSteps - 1) {
                    this.currentStep += 1;
                }
            },
            prevStep() {
                if (this.currentStep > 0) {
                    this.currentStep -= 1;
                }
            },
            toggleNode(node) {
                node.isExpanded = !node.isExpanded;
            },
            handleCheckboxChange(node) {
                if (node.isChecked) {
                    Vue.set(this.checkedNodes, this.checkedNodes.length, node);
                } else {
                    const index = this.checkedNodes.findIndex(item => item.id === node.id);
                    if (index !== -1) {
                        Vue.delete(this.checkedNodes, index);
                    }
                }
            },
            getTomorrowDate() {
                const today = new Date();
                const tomorrow = new Date(today.getFullYear(), today.getMonth(), today.getDate() + 1);
                return tomorrow;
            },
            handleSourceLanguageChange(sourceLanguage) {
                
                if (this.selectedSourceLanguage.includes(sourceLanguage.code)) {
                    this.selectedSourceLanguage = [];
                } else {
                    this.selectedSourceLanguage = [sourceLanguage.code];
                }
            },
            handleTargetLanguageChange(selectedLanguage) {
                this.selectedTargetLanguageNames.push(selectedLanguage.name);
            },
            confirmProject() {
                const request = {
                    integrationType: "string",
                    name: this.projectName,
                    sourceLanguage: this.selectedSourceLanguage[0],
                    targetLanguages: this.selectedTargetLanguages,
                    dueDate: this.formatDeadline(),
                    projectTemplateId: null,
                    selectedItemIds: this.checkedNodes.map(node => node.id),
                    selectedWorkflowStage: this.selectedWorkflowStage
                };
                console.log(request);
                axios.post('/api/basket/save-project', request)
                    .then(response => {

                        window.parent.$('.ui-dialog-content:visible').dialog('close');
                    })
                    .catch(error => {
                        alert("There was an error: " + error.response.data.Message);
                    });
            },
            cancel() {
                window.parent.$('.ui-dialog-content:visible').dialog('close');
            },
            formatDeadline() {
                if (!this.deadlineDate || !this.deadlineTime) {
                    return "";
                }

                const dateTimeString = `${this.deadlineDate} ${this.deadlineTime}`;
                const isoDateTime = moment(dateTimeString, "YYYY-MM-DD hh:mm A").toISOString();

                const options = {
                    year: 'numeric',
                    month: 'short',
                    day: 'numeric',
                    hour: 'numeric',
                    minute: 'numeric',
                    timeZoneName: 'short'
                };

                return new Intl.DateTimeFormat('en-US', options).format(new Date(isoDateTime));
            },
            generateTimeOptions() {
                // Генерация значений времени с интервалом в 30 минут в формате AM/PM
                const times = [];
                for (let hour = 0; hour < 24; hour++) {
                    for (let minute = 0; minute < 60; minute += 30) {
                        const time = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`;
                        const formattedTime = moment(time, 'HH:mm').format('hh:mm A'); // Преобразуем в формат AM/PM
                        times.push({
                            value: time, // Сохраняем исходное значение
                            label: formattedTime // Отображаемое значение в формате AM/PM
                        });
                    }
                }
                this.timeOptions = times;
            },
            handleSelectedLanguages() {
                console.log(this.selectedTargetLanguages);
                console.log(this.selectedSourceLanguage);
                let target = this.targetLanguages.filter(l => this.selectedTargetLanguages.includes(l.code));
                let source = this.sourceLanguages.filter(l => this.selectedSourceLanguage.includes(l.code));
                this.selectedTargetLanguageNames = target.map(l => l.name);
                this.selectedSourceLanguageName = source.map(l => l.name)[0];
            }
        }
    });
</script>

</body>
</html>
