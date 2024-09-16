<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthorizationModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.Smartcat.Authorization.AuthorizationModal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="https://cdn.jsdelivr.net/npm/vue@2"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link href="../closeButton.css" rel="stylesheet" type="text/css" />
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <script src="../onload.js"></script>
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700&display=swap" rel="stylesheet">
</head>
<body>
    <div id="app">
        <el-container>
            <el-button
                icon="el-icon-close"
                class="close-button"
                @click="closeWindow">
            </el-button>
        </el-container>
        <el-container class="center-container">
            <div class="input-wrapper">
                <el-row style="padding: 10px">
                    <el-input 
                        placeholder="Workspace Id"
                        v-model="workspaceId"
                        />
                </el-row>
                <el-row style="padding: 10px">
                    <el-input 
                        placeholder="ApiKey"
                        v-model="apiKey"
                        />
                </el-row>
                <el-button
                    @click="saveApiKey">
                    Submit
                </el-button>
            </div>
        </el-container>
    </div>
    <script>

        new Vue({
            el: '#app',
            data: {
                workspaceId: "",
                apiKey: ""
            },
            computed: {
            },
            created() {
            },
            methods: {
                saveApiKey() {
                    const data = {
                        workspaceId: this.workspaceId,
                        apiKey: this.apiKey
                    };

                    axios.post('/api/auth/save-apikey', data)
                        .then(response => {

                        })
                        .catch(error => {
                            alert(error.message);
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
