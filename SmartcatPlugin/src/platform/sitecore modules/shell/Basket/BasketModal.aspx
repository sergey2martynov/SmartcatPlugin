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
                <el-container>
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
                                    <template v-if="data.ShowCheckbox">
                                        <el-checkbox :checked="data.checked" disabled />
                                    </template>
                                    <img :src="data.ImageUrl" alt="" class="tree-node-icon">
                                    <span>{{ data.Name }}</span>
                                </div>
                            </span>
                        </el-tree>
                    </el-main>
                    <el-aside width="200px" style="padding: 10px; box-sizing: border-box;">
                        <div>In this will be validation info</div>
                    </el-aside>
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
            defaultProps: {
                children: 'Children',
                label: 'Name'
            }
        },
        computed: {
            processedTreeData() {
                const processNode = node => {
                    const processedNode = { ...node };
                    if (processedNode.ShowCheckbox) {
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
            this.fetchTreeData();
        },
        methods: {
            fetchTreeData() {
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
            nextStep() {
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
            }
        },
        mounted() {
            Vue.use(ELEMENT);
        },
        
    });
</script>
</body>
</html>
