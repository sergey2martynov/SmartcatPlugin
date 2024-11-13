<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectListModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.Smartcat.Projects.ProjectsListModal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8">
    <title>Add item</title>
    <script src="https://cdn.jsdelivr.net/npm/vue@2"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link href="../common.css" rel="stylesheet" type="text/css"/>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans:wght@400;600&display=swap" rel="stylesheet">
</head>
<body>
    <div id="app">
        <div class="container">
            <div class="tool-panel" style="font-size: 12px;">
                <div class="sort-section">
                    <label for="sort-select">Sort by</label>
                    <el-select id="sort-select"
                               v-model="selectedSortProperty"
                               style="font-size: 12px;">
                        <el-option v-for="item in sortProperties"
                                   style="font-size: 12px;"
                                   :key="item.id"
                                   :label="item.name"
                                   :value="item.id">
                        </el-option>
                    </el-select>
                </div>
                <div class="search-input">
                    <el-input 
                        v-model="searchValue"
                        placeholder="Search"
                        type="text"/>
                </div>
                <div class="translate-all-button-wrapper">
                    <button class="common-button cancel-button" style="width: 200px; font-size: 12px;">Get translation for all projects</button>
                </div>
            </div>
            <div class="table-header" style="padding: 9.2px 10px;">
                <span style="width: 342px;"> Project name </span>
                <span style="width: 200px;"> Languages </span>
                <span style="width: 100px;">Status</span>
                <span style="width: 160px;">Creation date</span>
                <span style="width: 68px;"></span>
            </div>
                <div class="project-list">
                    <div class="project" v-for="project in projects" :key="project.id" 
                         :style="{ 
                                    height: project.isExpanded ? 'auto' : '35.2px',
                                    paddingTop: project.isExpanded ? '10px' : '0'
    
                                 }">
                        <div class="project-header"
                             :style="{ 
                                       paddingTop: project.isExpanded ? '0' : '10px'
                                
                                     }">
                            <svg @click="toggleProject(project)"  v-if="!project.isExpanded" width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M6 12.5L6 3.5L11 8L6 12.5Z" fill="black"/>
                            </svg>
                            <svg @click="toggleProject(project)"  v-else width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M5 10.9999L10.9999 5L11 11L5 10.9999Z" fill="black"/>
                            </svg>
                            <span @click="toggleProject(project)"  style="width: 315px; padding-left: 10px;">{{ project.name }}</span>
                            <span @click="toggleProject(project)"  style="width: 200px;">{{ project.languages }}</span>
                            <span @click="toggleProject(project)"  style="width: 100px;" :style="{ 
                                                                  color: getStatusColor(project.status) 
                                                                }">
                                {{ project.status }}
                            </span>
                            <spa @click="toggleProject(project)" n style="width: 160px;">{{ project.creationDate }}</spa>
                            <span style="width: 68px;">
                                <svg style="padding-right: 15px;" @click="getProjectTranslation(project)" width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path d="M13.6564 2.05078L5.20648 10.5007M5.20648 10.5007L9.85781 10.5007M5.20648 10.5007V5.84936M13.6564 13.9508H2.34375" stroke="#474747" stroke-width="1.5" stroke-linecap="square"/>
                                </svg>
                                <svg @click="deleteProject(project)"width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path d="M2.71094 4.69523H13.2887M6.6776 7.33966V11.3063M9.32211 7.33966V11.3063M3.37212 4.85568L4.03319 12.6286C4.03319 12.9792 4.1725 13.3155 4.42046 13.5635C4.66843 13.8115 5.00474 13.9508 5.35542 13.9508H10.6443C10.995 13.9508 11.3313 13.8115 11.5793 13.5635C11.8272 13.3155 11.9665 12.9792 11.9665 12.6286L12.6277 4.85568M6.01652 4.69522V2.71189C6.01652 2.53655 6.08618 2.3684 6.21016 2.24442C6.33414 2.12043 6.5023 2.05078 6.67763 2.05078H9.32208C9.49741 2.05078 9.66557 2.12043 9.78955 2.24442C9.91354 2.3684 9.98319 2.53655 9.98319 2.71189V4.69522" stroke="#474747" stroke-width="1.5" stroke-linecap="square"/>
                                </svg>
                            </span>
                        </div>
                        <div v-if="project.isExpanded" style="margin-top: 10px;">
                            <div class="table-header" style="padding: 9.2px 0;">
                                <span style="width: 24px;"></span>
                                <span style="width: 318px;"> Page </span>
                                <span style="width: 200px;"> Language </span>
                                <span style="width: 292px;"> Status </span>
                                <span style="width: 36px;"></span>
                            </div>
                            <div class="document" v-for="doc in project.documents" :key="doc.id">
                                <span style="padding-left: 24px; width: 338px;">{{ doc.name }}</span>
                                <span style="width: 200px;">English test</span>
                                <span style="width: 292px;" :style="{ 
                                                                      color: getStatusColor(doc.status) 
                                                                    }" >
                                    {{ doc.status }}</span>
                                <span style="width: 68px;">
                                    <svg style="padding-right: 15px; cursor: pointer;" width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M13.6564 2.05078L5.20648 10.5007M5.20648 10.5007L9.85781 10.5007M5.20648 10.5007V5.84936M13.6564 13.9508H2.34375" stroke="#474747" stroke-width="1.5" stroke-linecap="square"/>
                                    </svg>
                                    <svg @click="deleteDocument(doc, project)"width="16" style="cursor: pointer;" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M2.71094 4.69523H13.2887M6.6776 7.33966V11.3063M9.32211 7.33966V11.3063M3.37212 4.85568L4.03319 12.6286C4.03319 12.9792 4.1725 13.3155 4.42046 13.5635C4.66843 13.8115 5.00474 13.9508 5.35542 13.9508H10.6443C10.995 13.9508 11.3313 13.8115 11.5793 13.5635C11.8272 13.3155 11.9665 12.9792 11.9665 12.6286L12.6277 4.85568M6.01652 4.69522V2.71189C6.01652 2.53655 6.08618 2.3684 6.21016 2.24442C6.33414 2.12043 6.5023 2.05078 6.67763 2.05078H9.32208C9.49741 2.05078 9.66557 2.12043 9.78955 2.24442C9.91354 2.3684 9.98319 2.53655 9.98319 2.71189V4.69522" stroke="#474747" stroke-width="1.5" stroke-linecap="square"/>
                                    </svg>
                                </span>
                            </div>
                        </div>
                    </div>
            </div>
            <div class="footer">
                <div class="footer-content">
                    <div style="height: 28px; align-content: center; padding-right: 10px;">
                        <a>Showing {{ currentPage * 10 + 1 }} - {{ currentPage * 10 + 10 }} projects</a>
                    </div>
                    <div style="height: 28px;">
                        <svg @click="previousPage" cursor="pointer" width="28" height="28" viewBox="0 0 28 28" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M14.7849 10.4297L11.2148 13.9997L14.7849 17.5697" stroke="#C0C0C0" stroke-width="1.2" stroke-linecap="square"/>
                        </svg>
                    </div>
                    <div style="height: 28px;">
                        <svg @click="nextPage" cursor="pointer" width="28" height="28" viewBox="0 0 28 28" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M13.2148 17.5697L16.7849 13.9997L13.2148 10.4297" stroke="#C0C0C0" stroke-width="1.2" stroke-linecap="square"/>
                        </svg>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        new Vue({
            el: '#app',
            data: {
                projects: [],
                sortProperties: [
                    {
                        name: "Creation date, newest first",
                        id: 0
                    },
                    {
                        name: "Creation date, oldest first",
                        id: 1
                    },
                    {
                        name: "Project name, alphabetically",
                        id: 2
                    },
                    {
                        name: "Project name, reverse alphabetically",
                        id: 3
                    }
                ],
                selectedSortProperty: 0,
                searchValue: '',
                currentPage: 0
            },
            created() {
                this.loadProjects();
            },
            methods: {
                loadProjects() {
                    axios.get('/api/project/get-projects?currentPageNumber=' + this.currentPage)
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
                    this.currentPage += 1;
                    this.loadProjects();
                },
                previousPage() {
                    if (this.currentPage == 0) {
                        return;
                    }

                    this.currentPage -= 1;
                    this.loadProjects();
                },
                getProjectTranslation(project) {
                    axios.get('/api/project/get-item-translations?projectId=' + project.id)
                        .then(response => {
                            console.log(response);
                            alert("Success: " + response.data);
                        })
                        .catch(error => {
                            alert('There was an error!', error.response.data.Message);
                        });
                },
                toggleProject(project) {
                    project.isExpanded = !project.isExpanded;

                    if (project.isExpanded) {
                        axios.get('/api/project/get-documents?projectId=' + project.id)
                            .then(response => {
                                project.documents = response.data.documents;
                                console.log(this.projects);
                            })
                            .catch(error => {
                                alert('There was an error!', error.response.data.Message);
                            });
                    } else {
                        console.log('Проект был свернут:', project.name);
                    }
                },
                getStatusColor(status) {
                    switch (status) {
                    case 'Completed':
                        return 'green';
                    case 'In progress':
                        return '#4381B5';
                    case 'Failed':
                        return 'red';
                    default:
                        return 'black';
                    }
                },
                deleteProject(project) {
                    axios.delete('/api/project/delete-project?id=' + project.id)
                        .then(response => {
                            this.loadProjects();
                        })
                        .catch(error => {
                            alert('There was an error!', error.response.data.Message);
                        });
                },
                deleteDocument(document, project) {
                    axios.delete(`/api/project/delete-document?id=${document.id}&projectId=${project.id}`)
                        .then(response => {
                            project.documents = project.documents.filter(doc => doc.id !== document.id);
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
