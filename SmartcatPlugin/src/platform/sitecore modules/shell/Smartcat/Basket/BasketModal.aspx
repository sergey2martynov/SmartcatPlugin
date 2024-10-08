<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasketModal.aspx.cs" 
Inherits="SmartcatPlugin.sitecore_modules.shell.Smartcat.Basket.BasketModal" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Basket</title>
    <script src="https://cdn.jsdelivr.net/npm/vue@2"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <script src="https://unpkg.com/element-ui/lib/umd/locale/en.js"></script>
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://unpkg.com/vue2-datepicker/index.css">
    <script src="https://unpkg.com/vue2-datepicker"></script>
    <script src="../onload.js"></script>
    <link href="../closeButton.css" rel="stylesheet" type="text/css" />
    <style>
        body, html {
            height: 100%;
            margin: 0;
            padding: 0;
            font-family: sans-serif;
            overflow: hidden;
        }
        #app {
            display: flex;
            flex-direction: column;
            height: 100%;
        }
        .custom-stepper {
            height: 100px;
        }

        .custom-stepper .el-step {
            padding-bottom: 15px;
        }

        .custom-stepper .el-step__head {
            border-color: transparent;
        }

        .custom-stepper .el-step__line {
            display: none;
        }

        .custom-stepper .el-steps__step {
            margin-bottom: 10px;
        }
        .el-step__head.is-process .el-step__icon {
            background-color: #7e3ff2 !important;
            border-color: #7e3ff2 !important;
            color: white !important;
            width: 20px;  
            height: 20px;
            font-size: 13px; 
            line-height: 20px; 
        }

        .el-step__head.is-finish .el-step__icon {
            background-color: #d3d3d3 !important;
            border-color: #d3d3d3 !important;
            color: gray !important;
            width: 20px;  
            height: 20px;
            font-size: 13px; 
            line-height: 20px;
        }

        .el-step__title.is-process {
            color: #7e3ff2 !important;
        }

        .el-step__title.is-wait {
            color: #d3d3d3 !important;
        }

        .el-step__title.is-finish {
            color: #d3d3d3 !important;
        }

        .el-step__icon-inner.is-finish {
            color: white;
        }
        .el-step__icon-inner.is-process {
            color: white;
        }
        .el-step__icon-inner.is-process {
            color: black;
        }

        .el-step__head.is-wait .el-step__icon {
            background-color: #d3d3d3 !important;
            border-color: #d3d3d3 !important;
            color: gray !important;
            width: 20px;  
            height: 20px;
            font-size: 13px; 
            line-height: 20px;
        }

        .el-step__head.is-wait .el-step__title {
            color: black !important;
        }

        .tree {
            flex-grow: 1;
            margin-right: 15px;
            max-height: 410px;
            overflow-y: auto;
        }

        .tree-node {
            height: 30px;
            display: flex;
            align-items: center;
            padding: 5px 10px;
            border-bottom: 1px solid #f0f0f0;
        }

        .tree-node img {
            width: 16px;
            height: 16px;
            padding-right: 10px;
        }

        .tree-node .toggle-icon {
            cursor: pointer;
            padding-right: 10px;
        }

        .tree-node-content {
            display: flex;
            align-items: center;
            flex-grow: 1;
            color: gray;
        }

        .tree-children {
            margin-left: 20px;
            display: none;
        }

        .tree-children.active {
            display: block;
        }

        .header {
            margin-top: 35px;
        }

        .checkbox {
            padding-right: 10px;
        }


        .el-checkbox.is-checked .el-checkbox__inner {
            background-color: #d3d3d3 !important;
            border-color: #d3d3d3 !important;
        }

        .el-checkbox__inner::after {
            border-color: white !important;
        }

        .aside-with-divider {
            position: relative;
            width: 200px !important;
            padding: 10px;
            box-sizing: border-box;
            margin-top: 35px;
            margin-left: 35px;
        }

        .aside-with-divider::after {
            content: '';
            position: absolute;
            top: 10px;
            bottom: 45px;
            right: 0;
            width: 1px;
            background-color: #d3d3d3;
        }

        .content {
            display: flex;
            flex-direction: column;
            gap: 10px;
            padding-bottom: 15px;
            margin-left: 30px;
        }

        .footer-container {
            height: 80px;
            display: flex;
            margin-top: auto;
            justify-content: space-between;
            align-items: center;
            padding: 10px 30px;
        }

        .left-buttons {
            display: flex;
            align-items: center;
        }

        .right-buttons {
            display: flex;
            align-items: center;
        }

        .cancel-button,
        .back-button,
        .next-button,
        .confirm-button {
            border: 1px solid black;
            width: 80px;
            height: 35px;
            margin-bottom: 30px;
            justify-content: center;
            align-items: center;
        }

        .cancel-button,
        .back-button {
            background-color: white;
            color: black;
        }

        .cancel-button:hover,
        .back-button:hover {
            background-color: #f0f0f0;
            border-color: black;
            color: #333;
        }

        .next-button,
        .confirm-button {
            background-color: black;
            color: white;
        }

        .next-button:hover,
        .confirm-button:hover
        {
            background-color: #333;
            border-color: black;
            color: white;
        }

        .cancel-button:focus,
        .back-button:focus,
        .next-button:focus,
        .confirm-button:focus,
        .cancel-button:active,
        .back-button:active,
        .next-button:active,
        .confirm-button:active {
            outline: none;
            background-color: inherit;
            color: inherit;
            border-color: inherit;
        }

        .next-button:focus,
        .confirm-button:focus,
        .next-button:active,
        .confirm-button:active {
            border-color: black;
            background-color: black;
            color: white;
        }


        .project-field-label {
            font-weight: bold;
        }

        .project-field {
            padding-top: 8px;
            margin-right: 35px;
        }

        .required {
            color: red;
        }

        .el-select .el-tag {
            color: black;
        }

        .project-field .mx-datepicker .mx-input-wrapper {
            height: 40px;
        }

        .el-textarea__inner {
            resize: none !important;
        }
    </style>
</head>
<body>
        <div id="app">
            <el-button
                icon="el-icon-close"
                class="close-button"
                @click="closeWindow">
            </el-button>
        <el-container>
            <el-aside class="aside-with-divider">
                <el-steps direction="vertical" :active="currentStep" class="custom-stepper" process-status="process" finish-status="finish">
                    <el-step v-for="(step, index) in steps" :key="index" :title="step.title" />
                </el-steps>
            </el-aside>
            <el-container direction="vertical">
                <div v-if="currentStep === 0" class="content">
                    <div>
                        <h2 class="header">
                            Content
                        </h2>
                    </div>
                    <div>
                        <h4>
                            Invalid items count: {{invalidItemCount}}
                        </h4>
                    </div>
                    <div class="tree">
                        <div v-for="node in treeData" :key="node.id">
                            <div class="tree-node">
                                <span class="toggle-icon" @click="toggleNode(node)">
                                    <i :class="'el-icon-caret-bottom'"></i>
                                </span>
                                <el-checkbox v-if="node.showCheckBox" disabled class="checkbox" v-model="node.isChecked">
                                </el-checkbox>
                                <div class="tree-node-content">
                                    <img :src="node.imageUrl" alt="icon">
                                    <span>{{ node.name }}</span>
                                </div>
                            </div>
                            <div class="tree-children active">
                                <tree-node v-if="node.children" :nodes="node.children"></tree-node>
                            </div>
                        </div>
                    </div>
                </div>
                <div v-if="currentStep === 1" class="content">
                        <div>
                            <h2 class="header">
                                Project
                            </h2>
                        </div>
                        <div>
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
                                        size="medium"
                                        
                                    />
                                </div>
                        </div>
                        <div>
                            <div>
                                <el-label class="project-field-label" for="workflowStagesSelect">
                                    Workflow Stages <span class="required">*</span>
                                </el-label>
                            </div>
                            <div class="project-field">
                                    <el-select
                                        v-model="selectedWorkflowStages"
                                        multiple
                                        placeholder="Select workflow stages"
                                        size="medium"
                                        @remove-tag="handleTagRemove"
                                        style="width: 100%">
                                        <el-option
                                            v-for="item in workflowStages"
                                            :key="item.id"
                                            :label="item.name"
                                            :value="item.name">
                                        </el-option>
                                    </el-select>
                            </div>
                        </div>
                        <div>
                            <div>
                                <el-label class="project-field-label">
                                    Deadline
                                </el-label>
                            </div>
                            <div class="project-field">
                                <el-date-picker
                                    v-model="deadline"
                                    type="datetime"
                                    placeholder="Set a deadline"
                                    size="medium"
                                    style="width: 100%"
                                    default-time="12:00:00"
                                    >
                                </el-date-picker>
                            </div>
                        </div>
                        <div>
                            <div>
                                <el-label class="project-field-label">
                                    Description <span class="required">*</span>
                                </el-label>
                            </div>
                            <div class="project-field">
                                <el-input 
                                    type="textarea" 
                                    id="description" 
                                    v-model="description"
                                    rows="4"
                                    :autosize="{ minRows: 4, maxRows: 4 }"
                                    style="width: 100%;
                                           height: 100px;"
                                />
                            </div>
                        </div>
                </div>
                <el-container v-if="currentStep === 2" style="margin-left: 30px; margin-right: 30px; display: grid; grid-template-rows: auto 1fr; grid-template-columns: 1fr 1fr; gap: 20px;">
                    <!-- Текстовая область -->
                    <el-container style="grid-column: 1 / span 2; text-align: center;">
                        <el-col>
                            <el-row style="text-align: left;">
                                <div style="height: 40px; background-color: gray; margin-top: 15px; display: flex; align-items: center;">
                                    <label style="color: aliceblue; margin-left: 10px">
                                        Translation
                                    </label>
                                </div>
                            </el-row>
                            <el-row style="padding-top: 15px; text-align: left;">
                                <label>
                                    Choose the target language(s) for your project. Depending on the service, 
                                    some languages may be unavailable or selected by default
                                </label>
                            </el-row>
                        </el-col>
                        
                    </el-container>

                    <!-- Колонка для исходных языков -->
                    <el-col>
                        <label style="padding-top: 15px; font-weight: bold;" >Source language</label>
                        <el-row style="padding-top: 15px" v-for="sourceLanguage in sourceLanguages" :key="sourceLanguage.code">
                            <el-checkbox
                                v-model="selectedSourceLanguage"
                                :label="sourceLanguage.code"
                                @change="handleSourceLanguageChange(sourceLanguage)"
                            >
                                {{ sourceLanguage.name }}
                            </el-checkbox>
                        </el-row>
                    </el-col>

                    <!-- Колонка для целевых языков -->
                    <el-col>
                        <label style="padding-top: 15px; font-weight: bold;">Target language</label>
                        <el-row style="padding-top: 15px" v-for="targetLanguage in targetLanguages" :key="targetLanguage.code">
                            <el-checkbox
                                v-model="selectedTargetLanguages"
                                :label="targetLanguage.code"
                                @change="handleTargetLanguageChange(targetLanguage)"
                            >
                                {{ targetLanguage.name }}
                            </el-checkbox>
                        </el-row>
                    </el-col>
                </el-container>
                <el-container v-if="currentStep === 3" style="margin-left: 30px; display: grid; grid-template-rows: auto 1fr; grid-template-columns: 200px 350px; gap: 20px;">
                    <el-container style="grid-column: 1 / span 2; width: 100%;">
                        <div style="height: 40px; background-color: gray; margin-top: 15px; width: 100%; display: flex; align-items: center;">
                            <label style="color: aliceblue; margin-left: 10px">
                                1 project created & added for translation
                            </label>
                        </div>
                    </el-container>
                        <el-col style="text-align: left;">
                            <el-row>
                                <label style="text-align: left; font-weight: bold;">
                                    Project name
                                </label>
                            </el-row>
                            <el-row>
                                <label style="text-align: left; font-weight: bold;">
                                    Workflow stage
                                </label>
                            </el-row>
                            <el-row>
                                <label style="text-align: left; font-weight: bold;">
                                    Deadline
                                </label>
                            </el-row>
                            <el-row>
                                <label style="text-align: left; font-weight: bold;">
                                    Description
                                </label>
                            </el-row>
                            <el-row>
                                <label style="text-align: left; font-weight: bold;">
                                    Source language
                                </label>
                            </el-row>
                            <el-row>
                                <label style="text-align: left; font-weight: bold;">
                                    Target language
                                </label>
                            </el-row>
                        </el-col>
                        <el-col style="text-align: left;">
                            <el-row>
                                <a style="padding-left: 30px">
                                    {{ projectName }}
                                </a>
                            </el-row>
                            <el-row>
                                <a style="padding-left: 30px">
                                    {{ selectedWorkFlowStage }}
                                </a>
                            </el-row>
                            <el-row>
                                <a style="padding-left: 30px">
                                    {{ new Date(deadline).toLocaleDateString() }}
                                </a>
                            </el-row>
                            <el-row>
                                <a style="padding-left: 30px">
                                    {{ description }}
                                </a>
                            </el-row>
                            <el-row>
                                <a style="padding-left: 30px">
                                    {{ selectedSourceLanguageName }}
                                </a>
                            </el-row>
                            <el-row>
                                <a style="padding-left: 30px">
                                    {{ selectedTargetLanguageNames.join(', ') }}
                                </a>
                            </el-row>
                        </el-col>
                </el-container>
            <el-footer class="footer-container">
                <div class="left-buttons">
                    <el-button
                        class="cancel-button"
                        v-if="currentStep === 0"
                        @click="cancel">
                        Cancel
                    </el-button>
                    <el-button
                        class="back-button"
                        v-else
                        @click="prevStep">
                        Back
                    </el-button>
                </div>
                <div class="right-buttons">
                    <el-button
                        v-if="currentStep < 3"
                        class="next-button"
                        @click="nextStep">
                        Next
                    </el-button>
                    <el-button
                        v-if="currentStep === 3"
                        class="confirm-button"
                        @click="confirmProject">
                        Confirm project
                    </el-button>
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
                <el-checkbox v-if="node.showCheckBox" disabled class="checkbox" v-model="node.isChecked" >
                </el-checkbox>
                <div class="tree-node-content">
                    <img :src="node.imageUrl" alt="icon">
                    <span>{{ node.name }}</span>
                </div>
            </div>
            <div class="tree-children" :class="{ active: node.isExpanded }">
                <tree-node v-if="node.children" :nodes="node.children" @toggle-node="$emit('toggle-node', $event)"></tree-node>
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
                { title: 'Content' },
                { title: 'Project' },
                { title: 'Languages' },
                { title: 'Confirmation' }
            ],
            totalSteps: 4,
            treeData: [],
            checkedNodes: [],
            allNodeIds: [],
            invalidItemNames: "",
            invalidItemCount: 0,
            validItemCount: 0,
            defaultProps: {
                children: 'children',
                label: 'name'
            },

            projectName: "",
            selectedWorkflowStages: [],
            deadline: '',
            description: "",
            workflowStages: [
                {
                    name: "Manual translation",
                    id: 0
                },
                {
                    name: "AI Translation",
                    id: 1
                },
                {
                    name: "AI translation + post-editing",
                    id: 2
                }
            ],

            sourceLanguages: [],
            targetLanguages: [],
            selectedSourceLanguage: [],
            selectedTargetLanguages: [],
            selectedSourceLanguageName: "",
            selectedTargetLanguageNames: [],

            summaryData: new Map()
        },
        computed: {
            processedTreeData() {
                const processNode = node => {
                    const processedNode = { ...node };
                    if (processedNode.showCheckBox) {
                        processedNode.isChecked = true;
                    }
                    if (processedNode.children && processedNode.children.length) {
                        processedNode.children = processedNode.children.map(processNode);
                    }
                    return processedNode;
                };
                return this.treeData.map(processNode);
            }
        },
        created() {
            this.getTreeData();
            this.getValidatingInfo();
            this.getSavedProjectInfo();
            this.getLanguages();
            if (this.workflowStages.length > 0) {
                this.selectedWorkFlowStage = this.workflowStages[0].name;
            }
        },
        methods: {
            getTreeData() {
                axios.get('/api/basket/get-selected-items')
                    .then(response => {
                        this.treeData = response.data.TreeNodes;
                        this.checkedNodes = response.data.CheckedItems;
                        this.allNodeIds = response.data.ExpandedItems;
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            getValidatingInfo() {
                axios.get('/api/basket/get-validating-info')
                    .then(response => {
                        this.invalidItemNames = response.data.InvalidItemNames;
                        this.invalidItemCount = response.data.InvalidItemCount;
                        this.validItemCount = response.data.ValidItemCount;
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            getSavedProjectInfo() {
                axios.get('/api/basket/get-saved-project-info')
                    .then(response => {
                        this.projectName = response.data.projectName;
                        this.selectedWorkflowStages = response.data.workflowStages;
                        this.description = response.data.description;
                        if (response.data.Deadline) {
                            this.deadline = new Date(response.data.Deadline);
                        }
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            getLanguages() {
                axios.get('/api/basket/get-translation-languages')
                    .then(response => {
                        this.sourceLanguages = response.data.sourceLanguages;
                        this.targetLanguages = response.data.targetLanguages;
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            nextStep() {
                if (this.currentStep === 1) {

                    if (!this.projectName || !this.selectedWorkflowStages || !this.description) {
                        alert("Required fields was not filling");
                        return;
                    }

                    const data = {
                        projectName: this.projectName,
                        workflowStages: this.selectedWorkflowStages,
                        deadline: this.deadline ? this.deadline.toISOString() : null,
                        description: this.description
                    };

                    axios.post('/api/basket/save-project-info', data)
                        .then(response => {

                        })
                        .catch(error => {
                            alert("There was an error: " + error.message);
                        });
                }
                if (this.currentStep === 2) {
                    this.getSummaryData();
                }
                if (this.currentStep < this.totalSteps - 1) {
                    this.currentStep += 1;
                }
                this.$nextTick(() => {
                    console.log(`Step updated to: ${this.currentStep}`);
                });
            },
            prevStep() {
                if (this.currentStep > 0) {
                    this.currentStep -= 1;
                }
                this.$nextTick(() => {
                    console.log(`Step updated to: ${this.currentStep}`);
                });
            },
            getTomorrowDate() {
                const today = new Date();
                const tomorrow = new Date(today.getFullYear(), today.getMonth(), today.getDate() + 1);
                return tomorrow;
            },
            handleSourceLanguageChange(selectedLanguage) {
                this.selectedSourceLanguage = [];
                this.selectedSourceLanguageName = selectedLanguage.name;
                this.selectedSourceLanguage.push(selectedLanguage.code);
            },
            handleTargetLanguageChange(selectedLanguage) {
                this.selectedTargetLanguageNames.push(selectedLanguage.name);
            },
            getSummaryData() {
                const formatedDeadline = new Date(this.deadline).toLocaleDateString();
                this.summaryData = new Map([
                    ['Project name', this.projectName],
                    ['Workflow stage', this.selectedWorkFlowStage],
                    ['Deadline', formatedDeadline],
                    ['Description', this.description],
                    ['Source language', this.selectedSourceLanguage],
                    ['Target languages', this.selectedTargetLanguages.join(',')]]);
            },
            confirmProject() {
                const data = {
                    "integrationType": "string",
                    "name": this.projectName,
                    "description": this.description,
                    "sourceLanguage": this.selectedSourceLanguage[0],
                    "targetLanguage": this.selectedTargetLanguages[0],
                    "dueDate": this.deadline,
                    "projectTemplateId": null
                };

                axios.post('/api/basket/save-project', data)
                    .then(response => {

                        window.parent.$('.ui-dialog-content:visible').dialog('close');
                    })
                    .catch(error => {
                        alert("There was an error: " + error.response.data.Message);
                    });
            },
            closeWindow() {
                window.parent.$('.ui-dialog-content:visible').dialog('close');
            },
            toggleNode(node) {
                node.isExpanded = true;
            },
            cancel() {

            },
            handleTagRemove(tag) {
                console.log('Tag removed:', tag);
            },
            formatDeadline(deadline) {
                if (!deadline) return '';

                const options = {
                    year: 'numeric',
                    month: 'short',
                    day: 'numeric',
                    hour: 'numeric',
                    minute: 'numeric',
                    timeZoneName: 'short'
                };

                return new Intl.DateTimeFormat('en-US', options).format(new Date(deadline));
            }
        }
    });
</script>

</body>
</html>
