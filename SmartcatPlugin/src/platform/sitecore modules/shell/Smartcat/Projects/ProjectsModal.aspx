<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectsModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.Smartcat.Projects.ProjectsModal" %>

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
                            <th style="padding: 8px; border: 1px solid #ddd;">ID</th>
                            <th style="padding: 8px; border: 1px solid #ddd;">Status</th>
                            <th style="padding: 8px; border: 1px solid #ddd;">ProjectName</th>
                            <th style="padding: 8px; border: 1px solid #ddd;">Languages</th>
                            <th style="padding: 8px; border: 1px solid #ddd;">Author</th>
                            <th style="padding: 8px; border: 1px solid #ddd;">Words</th>
                            <th style="padding: 8px; border: 1px solid #ddd;">Created at</th>
                        </tr>
                        </thead>
                        <tbody>
                        <tr v-for="item in items" :key="item.id">
                            <td style="padding: 8px; border: 1px solid #ddd;">{{ item.id }}</td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{{ item.status }}</td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{{ item.projectName }}</td>
                            <td style="padding: 8px; border: 1px solid #ddd; white-space: pre-line;" v-html="item.languages"></td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{{ item.author }}</td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{{ item.words }}</td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{{ item.createdAt }}</td>
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
                items: [
                    { id: 5461546, status: 'In process', projectName: 'name 232311331', languages: 'Source: en<br>Target: ru, de, ru-BE, ru- BE, ru-BE', author: 'Evan You', words: 500, createdAt: '2021-01-01' },
                    { id: 9841645, status: 'Finished', projectName: 'name 232311331', languages: 'Source: en<br>Target: ru, de, ru-BE', author: 'Nicholas C. Zakas', words: 1500, createdAt: '2021-02-15' },
                    { id: 8468156, status: 'Finished', projectName: 'name 232311331', languages: 'Source: en<br>Target: ru, de, ru-BE', author: 'Kyle Simpson', words: 1200, createdAt: '2021-03-10' },
                    { id: 8416518, status: 'Finished', projectName: 'name 232311331', languages: 'Source: en<br>Target: ru, de, ru-BE', author: 'Marijn Haverbeke', words: 1100, createdAt: '2021-04-05' },
                    { id: 445166888, status: 'Finished', projectName: 'name 232311331', languages: 'Source: en<br>Target: ru, de, ru-BE', author: 'Andrew Hunt', words: 950, createdAt: '2021-05-20' }
                ]
            },
            created() {

            },
            methods: {
                closeWindow() {
                    window.parent.$('.ui-dialog-content:visible').dialog('close');
                },
                nextPage() {

                },
                previousPage() {

                }
            }
        });
    </script>
</body>
</html>
