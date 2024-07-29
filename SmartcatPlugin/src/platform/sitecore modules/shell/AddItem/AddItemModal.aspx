<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddItemModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.AddItem.AddItemModal" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>Add item</title>
    <script src="https://cdn.jsdelivr.net/npm/vue@2"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link href="styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
<div id="app">
    <el-container style="height: 100vh;">
        <el-aside width="200px" style="background-color: #505050; padding: 10px; box-sizing: border-box;"/>
        <el-container direction="vertical">
            <el-container>
                <el-main class="tree-container">
                    <el-tree
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

            }
        });
</script>
</body>
</html>
