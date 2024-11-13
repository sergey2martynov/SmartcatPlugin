<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthorizationModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.Smartcat.Authorization.AuthorizationModal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="https://cdn.jsdelivr.net/npm/vue@2"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <link href="../common.css" rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700&display=swap" rel="stylesheet">
</head>
<body>
    <div id="app">
        <el-container class="center-container">
            <el-row class="input-wrapper">
                <el-row style="margin-top: 15px" class="label-wrapper">
                    <label>Account ID<span class="required">*</span></label>
                </el-row>
                <el-row style="padding-top: 10px">
                    <el-input 
                        placeholder="Enter an account ID"
                        v-model="workspaceId"
                        @change="validateWorkspaceId()"
                        />
                </el-row>
                <el-row v-if="!isValidWorkspaceId" class="required validating-info">
                    Enter a valid Account ID
                </el-row>
                <el-row style="padding-top: 15px" class="label-wrapper">
                    <label>API key<span class="required">*</span></label>
                </el-row>
                <el-row style="padding-top: 25px">
                    <el-input 
                        placeholder="Enter an API Key"
                        v-model="apiKey"
                        :type="inputType"
                        @change="validateApiKey()"
                        />
                </el-row>
                <el-row v-if="!isValidApiKey" class="required validating-info">
                    Enter a valid API Key
                </el-row>
                <el-row style="padding-top: 10px; padding-bottom: auto;" class="text-wrapper">
                    Navigate to the <a href="https://smartcat.com/settings/api" target="_blank">“Settings / API”</a> section of the desired
                        workspace to find the Account ID and create an API key.
                </el-row>
            </el-row>
            <el-row class="button-wrapper">
                <div class="loader-wrapper">
                    <div v-if="isLoading" class="loader"></div>
                </div>
                <button class="common-button submit-button"
                        @click="saveApiKey"
                        :disabled="isDisableConnectButton">
                    Connect
                </button>
                <button class="common-button cancel-button"
                        @click="closeWindow">
                    Cancel
                </button>
            </el-row>
        </el-container>
    </div>
    <script>

        new Vue({
            el: '#app',
            data: {
                workspaceId: "",
                apiKey: "",
                isValidWorkspaceId: true,
                isValidApiKey: true,
                isLoading: false,
                isDisableConnectButton: false,
                inputType: "text",
                isFirstTrying: true,
                isFieldValidated: false
            },
            computed: {
            },  
            created() {
                this.getApiKey();
                this.changeDisablingConnectButton();
            },
            methods: {
                getApiKey() {
                    axios.get('/api/auth/get-apikey')
                        .then(response => {
                            this.workspaceId = response.data.workspaceId;
                            this.apiKey = response.data.apiKey;

                            if (response.data.apiKey) {
                                this.inputType = "password";
                            }
                        });
                },
                saveApiKey() {
                    this.isLoading = true;

                    const data = {
                        workspaceId: this.workspaceId,
                        apiKey: this.apiKey
                    };

                    axios.post('/api/auth/save-apikey', data)
                        .then(response => {
                            window.parent.$('.ui-dialog-content:visible').dialog('close');
                        })
                        .catch(error => {
                            console.log(error);
                            this.isFirstTrying = false;
                            this.isValidWorkspaceId = false;
                            this.isValidApiKey = false;
                            this.isFieldValidated = false;
                            this.changeDisablingConnectButton();
                        })
                        .finally(() => {
                            this.isLoading = false;
                        });
                },
                validateWorkspaceId() {
                    
                    if (this.workspaceId.length === 36 || this.workspaceId.length === 0) {
                        console.log("validateWorkspaceId", this.workspaceId.length);
                        this.isValidWorkspaceId = true;
                        this.changeDisablingConnectButton();

                        if (!this.isFirstTrying && !this.isFieldValidated) {
                            this.isFieldValidated = true;
                            this.validateApiKey();
                        }

                        return;
                    }

                    this.isValidWorkspaceId = false;
                    this.changeDisablingConnectButton();
                },
                validateApiKey() {
                    this.inputType = "text";
                    if (this.apiKey.length === 27 || this.apiKey.length === 0) {
                        console.log("validateApiKey", this.apiKey.length);
                        this.isValidApiKey = true;
                        this.changeDisablingConnectButton();

                        if (!this.isFirstTrying && !this.isFieldValidated) {
                            this.isFieldValidated = true;
                            this.validateWorkspaceId();
                        }

                        return;
                    }

                    this.isValidApiKey = false;
                    this.changeDisablingConnectButton();
                },
                changeDisablingConnectButton() {
                    if (!this.isValidWorkspaceId ||
                        !this.isValidApiKey ||
                        this.apiKey.length === 0 ||
                        this.workspaceId.length === 0) {
                        this.isDisableConnectButton = true;
                        return;
                    }

                    this.isDisableConnectButton = false;
                },
                closeWindow() {
                    window.parent.$('.ui-dialog-content:visible').dialog('close');
                }
            }
        });
    </script>
</body>
</html>
