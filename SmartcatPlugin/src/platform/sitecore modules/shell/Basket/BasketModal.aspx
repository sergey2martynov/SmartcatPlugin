<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasketModal.aspx.cs" 
Inherits="SmartcatPlugin.sitecore_modules.shell.Basket.BasketModal" %>

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

</head>
<body>
        <div id="app">
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
                            node-key="Id"
                            check-strictly>
                            <span slot="default" slot-scope="{ node, data }">
                                <div class="tree-node-content">
                                    <template v-if="data.ShowCheckBox">
                                        <el-checkbox :checked="data.checked" disabled />
                                    </template>
                                    <img :src="data.ImageUrl" alt="" class="tree-node-icon">
                                    <span>{{ data.Name }}</span>
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
                                <el-label for="subject" >
                                    Subject*
                                </el-label>
                            </el-col>
                            <el-col style="width: 300px">
                                <el-input 
                                    type="text" 
                                    id="subject" 
                                    v-model="subject"
                                    size="small"
                                />
                            </el-col>
                        </el-row>
                        <el-row style="padding-top: 15px; ">
                            <el-col style="width: 150px; text-align: right; padding-right: 15px">
                                <el-label for="subject" >
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
                                <el-label for="description">
                                    Description
                                </el-label>
                            </el-col>
                            <el-col style="width: 300px;">
                                <el-input 
                                    type="textarea" 
                                    id="description" 
                                    v-model="description"
                                    rows="4"
                                    style="width: 100%; height: 100px;"
                                />
                            </el-col>
                        </el-row>
                    </el-col>
                </el-container>
                <el-footer style="text-align: center;">
                    <el-button style="width: 100px; height: 40px;" :disabled="currentStep === 0" type="primary" @click="prevStep">
                        Back
                    </el-button>
                    <el-button style="width: 100px; height: 40px;" :disabled="currentStep === 3" type="primary" @click="nextStep">
                        Next
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
            validItemCount:0,
            defaultProps: {
                children: 'Children',
                label: 'Name'
            },

            projectName: "",
            subject: "",
            selectedWorkFlowStage: "",
            deadline: "",
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
            ]
        },
        computed: {
            processedTreeData() {
                const processNode = node => {
                    const processedNode = { ...node };
                    if (processedNode.ShowCheckBox) {
                        processedNode.checked = true;
                    }
                    if (processedNode.Children && processedNode.Children.length) {
                        processedNode.Children = processedNode.Children.map(processNode);
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
                        this.projectName = response.data.ProjectName;
                        this.subject = response.data.Subject;
                        this.selectedWorkFlowStage = response.data.WorkflowStage;
                        this.deadline = new Date(response.data.Deadline);
                        this.description = response.data.Description;
                        console.log(this.deadline);
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            nextStep() {
                if (this.currentStep === 1) {

                    if (!this.projectName || !this.subject || !this.selectedWorkFlowStage || !this.deadline) {
                        alert("Required fields was not filling");
                        return;
                    }

                    const data = {
                        projectName: this.projectName,
                        subject: this.subject,
                        workflowStage: this.selectedWorkFlowStage,
                        deadline: this.deadline.toISOString(),
                        description: this.description
                    };
                    console.log(data);
                    axios.post('/api/basket/save-project-info', data)
                        .then(response => {
                            this.currentStep += 1;
                        })
                        .catch(error => {
                            alert("There was an error: " + error.message);
                        });
                }
                if (this.currentStep < this.totalSteps - 1 ) {
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
            }
        },
        mounted() {
            Vue.use(ELEMENT);
        },
        
    });
</script>
</body>
</html>
