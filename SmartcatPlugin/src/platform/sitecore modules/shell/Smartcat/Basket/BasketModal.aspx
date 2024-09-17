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
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://unpkg.com/vue2-datepicker/index.css">
    <script src="https://unpkg.com/vue2-datepicker"></script>
    <script src="../onload.js"></script>
    <link href="../closeButton.css" rel="stylesheet" type="text/css" />
</head>
<body>
        <div id="app">
            <el-button
                icon="el-icon-close"
                class="close-button"
                @click="closeWindow">
            </el-button>
        <el-container style="height: 100vh;">
            <el-aside width="200px" style="background-color: #505050; padding: 10px; box-sizing: border-box;">
                <div style="height: 35px; display: flex; align-items: center;">
                    <el-column>
                        <label>Content</label>
                    </el-column>
                    <el-column>
                        <label v-if="currentStep === 0"><</label>
                    </el-column>
                </div>
                <div style="height: 35px; display: flex; align-items: center;">
                    <el-column>
                        <label>Project</label>
                    </el-column>
                    <el-column>
                        <label v-if="currentStep === 1"><</label>
                    </el-column>
                </div>
                <div style="height: 35px; display: flex; align-items: center;">
                    <el-column>
                        <label>Language</label>
                    </el-column>
                    <el-column>
                        <label v-if="currentStep === 2"><</label>
                    </el-column>
                </div>
                <div style="height: 35px; display: flex; align-items: center;">
                    <el-column>
                        <label>Confirmation</label>
                    </el-column>
                    <el-column>
                        <label v-if="currentStep === 3"><</label>
                    </el-column>
                </div>
            </el-aside>
            <el-container direction="vertical">
                <el-container v-if="currentStep === 0">
                    <el-main class="tree-container">
                        <el-tree
                            ref="tree"
                            :data="processedTreeData"
                            :props="defaultProps"
                            :default-expanded-keys="allNodeIds"
                            node-key="id"
                            check-strictly>
                            <span slot="default" slot-scope="{ node, data }">
                                <div class="tree-node-content">
                                    <template v-if="data.showCheckBox">
                                        <el-checkbox :checked="data.isChecked" disabled />
                                    </template>
                                    <img :src="data.imageUrl" alt="" class="tree-node-icon">
                                    <span>{{ data.name }}</span>
                                </div>
                            </span>
                        </el-tree>
                    </el-main>
                    <el-aside width="200px" style="padding: 10px; box-sizing: border-box;">
                        <div style="display: flex; flex-direction: column;">
                            <a>{{validItemCount}} items are valid</a>
                            <div v-if="invalidItemCount > 0">
                                <a>{{invalidItemCount}} items failed</a>
                                <a>Invalid item names: {{invalidItemNames}}</a>
                            </div>
                        </div>
                    </el-aside>
                </el-container>
                <el-container v-if="currentStep === 1" style="margin-left: 30px">
                    <el-col>
                        <el-row style="margin-top: 15px;" >
                                <el-col style="width: 150px; text-align: right; padding-right: 15px">
                                    <el-label for="projectName">
                                        Project name*
                                    </el-label>
                                </el-col>
                                <el-col style="width: 300px">
                                    <el-input 
                                        type="text" 
                                        id="projectName" 
                                        v-model="projectName"
                                        size="small"
                                    />
                                </el-col>
                        </el-row>
                        <el-row style="padding-top: 15px">
                            <el-col style="width: 150px; text-align: right; padding-right: 15px">
                                <el-label for="workflowStagesSelect">
                                    Workflow Stages*
                                </el-label>
                            </el-col>
                            <el-col style="width: 300px;">
                                <el-select
                                    id="workflowStagesSelect"
                                    value-key="id"
                                    v-model="selectedWorkFlowStage"
                                    size="small"
                                    class="mr-1"
                                    style="width: 300px">
                                    <el-option
                                        v-for="item in workflowStages"
                                        :key="item.id"
                                        :label="item.name"
                                        :value="item.name">
                                    </el-option>
                                </el-select>
                            </el-col>
                        </el-row>
                        <el-row style="padding-top: 15px; ">
                            <el-col style="width: 150px; text-align: right; padding-right: 15px">
                                <el-label for="deadline" >
                                    Deadline*
                                </el-label>
                            </el-col>
                            <el-col style="width: 300px">
                                    <date-picker
                                        v-model="deadline"
                                        type="date"
                                        placeholder="Pick a date"
                                        :default-value="getTomorrowDate()"
                                        size="small"
                                        style="width: 300px"
                                    />
                            </el-col>
                        </el-row>
                        <el-row style="padding-top: 15px;">
                            <el-col style="width: 150px; text-align: right; padding-right: 15px;">
                                <el-label for="comment">
                                    Comment
                                </el-label>
                            </el-col>
                            <el-col style="width: 300px;">
                                <el-input 
                                    type="textarea" 
                                    id="comment" 
                                    v-model="comment"
                                    rows="4"
                                    style="width: 100%; height: 100px;"
                                />
                            </el-col>
                        </el-row>
                    </el-col>
                </el-container>
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
                                    Comment
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
                                    {{ comment }}
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
                <el-footer style="text-align: center;">
                    <el-button style="width: 100px; height: 40px;" :disabled="currentStep === 0" type="primary" @click="prevStep">
                        Back
                    </el-button>
                    <el-button v-if="currentStep < 3"style="width: 100px; height: 40px;" type="primary" @click="nextStep">
                        Next
                    </el-button>
                    <el-button v-if="currentStep === 3"style="width: 130px; height: 40px;" type="primary" @click="confirmProject">
                        Confirm project
                    </el-button>
                </el-footer>
            </el-container>
        </el-container>
    </div>
<script>
    new Vue({
        el: '#app',
        data: {
            currentStep: 0,
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
            selectedWorkFlowStage: "",
            deadline: "",
            comment: "",
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
                        console.log(response);
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
                        this.projectName = response.data.ProjectName;
                        this.selectedWorkFlowStage = response.data.WorkflowStage;
                        this.deadline = new Date(response.data.Deadline);
                        this.comment = response.data.Comment;
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            getLanguages() {
                axios.get('/api/basket/get-translation-languages')
                    .then(response => {
                        console.log(response);
                        this.sourceLanguages = response.data.sourceLanguages;
                        this.targetLanguages = response.data.targetLanguages;
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            nextStep() {
                if (this.currentStep === 1) {

                    if (!this.projectName || !this.selectedWorkFlowStage || !this.deadline) {
                        alert("Required fields was not filling");
                        return;
                    }

                    const data = {
                        projectName: this.projectName,
                        workflowStage: this.selectedWorkFlowStage,
                        deadline: this.deadline.toISOString(),
                        comment: this.comment
                    };
                    console.log(data);
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
                console.log('Selected source language:', this.selectedSourceLanguage);
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
                    ['Comment', this.comment],
                    ['Source language', this.selectedSourceLanguage],
                    ['Target languages', this.selectedTargetLanguages.join(',')]]);
            },
            confirmProject() {
                const data = {
                    "integrationType": "string",
                    "name": this.projectName,
                    "description": this.comment,
                    "sourceLanguage": this.selectedSourceLanguage[0],
                    "targetLanguage": this.selectedTargetLanguages[0],
                    "dueDate": this.deadline,
                    "projectTemplateId": null
                };

                axios.post('/api/basket/save-project', data)
                    .then(response => {
                        console.log(response);
                        if (!response.ok) {
                            return response.text().then(text => { throw new Error(text); });
                        }

                        window.parent.$('.ui-dialog-content:visible').dialog('close');
                    })
                    .catch(error => {
                        console.log("ERROR", error.response.data.Message);
                        alert("There was an error: " + error.response.data.Message);
                    });
            },
            closeWindow() {
                window.parent.$('.ui-dialog-content:visible').dialog('close');
            }
        }
    });
</script>
</body>
</html>
