<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddItemModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.AddItem.AddItemModal" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>Add item</title>
    <script src="https://cdn.jsdelivr.net/npm/vue@2"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <link href="../Basket/styles.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
</head>
<body>
<div id="app">
    <el-container style="height: 100vh;">
        <el-aside width="200px" style="background-color: #505050; padding: 10px; box-sizing: border-box;">
            Add items to basket
        </el-aside>
        <el-container direction="vertical">
            <el-container>
                <el-main class="tree-container" style="background-color: #fff;">
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
                                    <el-checkbox 
                                        :checked="data.checked" 
                                        @change="handleCheckboxChange(node, $event)" 
                                        @click.native.stop/>
                                </template>
                                <img :src="data.ImageUrl" alt="" class="tree-node-icon">
                                <span>{{ data.Name }}</span>
                            </div>
                        </span>
                    </el-tree>
                </el-main>
                <el-aside width="200px" style="padding: 10px; box-sizing: border-box;">
                    
                </el-aside>
            </el-container>
            <el-footer style="text-align: center;">
                <el-button style="width: 100px; height: 40px;" type="primary" @click="saveItems">
                    Save
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
                        if (this.checkedNodes.includes(processedNode.Id)) {
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
            },
            methods: {
                getTreeData() {
                    axios.get('/api/additem/get-items-tree')
                        .then(response => {
                            this.treeData = response.data.TreeNodes;
                            this.checkedNodes = response.data.CheckedItems;
                            this.allNodeIds = response.data.ExpandedItems;

                            this.$nextTick(() => {
                                this.$refs.tree.setCheckedKeys(this.checkedNodes);
                            });
                        })
                        .catch(error => {
                            console.error('There was an error!', error);
                        });
                },
                saveItems() {
                    const checkedNodes = this.$refs.tree.getCheckedNodes();
                    const checkedIds = checkedNodes.map(node => node.Id);
                    const data = {
                        SelectedItemIds: checkedIds
                    };

                    axios.post('/api/additem/add-items', data)
                        .then(response => {
                                
                            window.parent.$('.ui-dialog-content:visible').dialog('close');
                        })
                        .catch(error => {
                            console.error('There was an error!', error);
                        });
                },
                handleCheckboxChange(node, checked) {
                    node.checked = checked;
                }
            }
        });
</script>
</body>
</html>
