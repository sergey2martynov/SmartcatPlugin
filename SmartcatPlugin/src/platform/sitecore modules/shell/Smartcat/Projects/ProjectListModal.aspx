<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectListModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.Smartcat.Projects.ProjectsListModal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8">
    <title>Add item</title>
    <script src="https://cdn.jsdelivr.net/npm/vue@2"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <link href="../Basket/styles.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <script src="../onload.js"></script>
    <link href="../closeButton.css" rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700&display=swap" rel="stylesheet">
    <style>
        h1, th {
            font-family: 'Plus Jakarta Sans';
        }
    </style>
</head>
<body>
    <div id="app">
        <el-container>
            <el-header>
                <el-container>
                    <h1>
                        Translation requests
                    </h1>
                </el-container>
                <el-container>
                    <el-button
                        icon="el-icon-close"
                        class="close-button"
                        @click="closeWindow">
                    </el-button>
                </el-container>
            </el-header>
            <el-main>
                <el-container>
                    <table style="width: 100%; border-collapse: collapse;">
                        <thead>
                        <tr>
                            <th style="padding: 8px; border: 1px solid #ddd;">Status</th>
                            <th style="padding: 8px; border: 1px solid #ddd;">ProjectName</th>
                            <th style="padding: 8px; border: 1px solid #ddd;">Languages</th>
                            <th style="padding: 8px; border: 1px solid #ddd;">Created at</th>
                            <th style="padding: 8px; border: 1px solid #ddd;"></th>
                        </tr>
                        </thead>
                        <tbody>
                        <tr v-for="project in projects" :key="project.id">
                            <td style="padding: 8px; border: 1px solid #ddd;">{{ project.status }}</td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{{ project.name }}</td>
                            <td style="padding: 8px; border: 1px solid #ddd; white-space: pre-line;" v-html="project.languages"></td>
                            <td style="padding: 8px; border: 1px solid #ddd;">createdAt</td>
                            <td style="padding: 8px; border: 1px solid #ddd;">
                                <el-button @click="getProjectTranslation(project)">
                                    Get translation
                                </el-button>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                </el-container>
            </el-main>
            <el-footer>
                <a>Showing 5 of 13 projects
                    <span>
                        <i class="el-icon-caret-left" style="cursor: pointer;" @click="previousPage"></i>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <i class="el-icon-caret-right" style="cursor: pointer;" @click="nextPage"></i>
                    </span>
                </a>
            </el-footer>
        </el-container>
    </div>
    <script>
        new Vue({
            el: '#app',
            data: {
                projects:[]
            },
            created() {
                this.loadProjects(0);
            },
            methods: {
                loadProjects(offset) {
                    axios.get('/api/project/get-projects?offset=' + offset)
                        .then(response => {
                            console.log(response);
                            this.projects = response.data.projects;
                        })
                        .catch(error => {
                            alert('There was an error!', error.response.data.Message);
                        });
                },
                closeWindow() {
                    window.parent.$('.ui-dialog-content:visible').dialog('close');
                },
                nextPage() {

                },
                previousPage() {

                },
                getProjectTranslation(project) {
                    axios.get('/api/project/get-item-translations?projectId=' + project.id)
                        .then(response => {
                            console.log(response);
                            this.projects = response.data.Data.projects;

                        })
                        .catch(error => {
                            alert('There was an error!', error.response.data.Message);
                        });
                }
            }
        });
    </script>
</body>
</html>
