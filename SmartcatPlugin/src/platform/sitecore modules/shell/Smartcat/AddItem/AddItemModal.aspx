<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddItemModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.Smartcat.AddItem.AddItemModal" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Select Pages</title>
    <link href="https://unpkg.com/element-ui/lib/theme-chalk/index.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/vue@2"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <script src="../onload.js"></script>
    <link href="../closeButton.css" rel="stylesheet" type="text/css" />
    <link href="styles.css" rel="stylesheet" type="text/css"/>
</head>
<body>
<div id="app">
    <div>
        <h3 class="header">
            Select items for translations
        </h3>
        <el-button
            icon="el-icon-close"
            class="close-button"
            @click="closeWindow">
        </el-button>
    </div>
    <div class="tree">
        <div v-for="node in treeData" :key="node.id">
            <div class="tree-node">
                <span class="toggle-icon" @click="toggleNode(node)">
                    <i :class="node.isExpanded ? 'el-icon-caret-bottom' : 'el-icon-caret-right'"></i>
                </span>
                <el-checkbox v-if="node.showCheckBox" class="checkbox" v-model="node.isChecked" @change="handleCheckboxChange(node)">
                </el-checkbox>
                <div class="tree-node-content">
                    <img :src="node.imageUrl" alt="icon">
                    <span>{{ node.name }}</span>
                </div>
            </div>
            <div class="tree-children" :class="{ active: node.isExpanded }">
                <tree-node v-if="node.children" :nodes="node.children" @toggle-node="toggleNode" @handle-checkbox-change="handleCheckboxChange"></tree-node>
            </div>
        </div>
    </div>
    <el-footer>
        <div class="footer-container">
            <el-button class="cancel-button">
                Cancel
            </el-button>
            <el-button class="save-button" @click="saveItems">
                Save
            </el-button>
        </div>        
    </el-footer>
</div>

<script>
    Vue.component('tree-node', {
        props: ['nodes'],
        template: `
        <div>
            <div v-for="node in nodes" :key="node.id">
                <div class="tree-node">
                    <span class="toggle-icon" @click="$emit('toggle-node', node)">
                        <i :class="node.isExpanded ? 'el-icon-caret-bottom' : 'el-icon-caret-right'"></i>
                    </span>
                    <el-checkbox v-if="node.showCheckBox" class="checkbox" v-model="node.isChecked" @change="$emit('handle-checkbox-change', node)">
                    </el-checkbox>
                    <div class="tree-node-content">
                        <img :src="node.imageUrl" alt="icon">
                        <span>{{ node.name }}</span>
                    </div>
                </div>
                <div class="tree-children" :class="{ active: node.isExpanded }">
                    <tree-node v-if="node.children" :nodes="node.children" @toggle-node="$emit('toggle-node', $event)" @handle-checkbox-change="$emit('handle-checkbox-change', $event)"></tree-node>
                </div>
            </div>
        </div>
    `
    });

    new Vue({
        el: '#app',
        data: {
            treeData: [],
            checkedNodes: [],
            allNodeIds: [],
            defaultProps: {
                children: 'Children',
                label: 'Name'
            },
            checkedNodesForSaving: []
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
                    })
                    .catch(error => {
                        console.error('There was an error!', error);
                    });
            },
            saveItems() {
                const checkedIds = this.checkedNodes.map(node => node.id);
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
            closeWindow() {
                window.parent.$('.ui-dialog-content:visible').dialog('close');
            }
        }
    });
</script>
</body>
</html>
