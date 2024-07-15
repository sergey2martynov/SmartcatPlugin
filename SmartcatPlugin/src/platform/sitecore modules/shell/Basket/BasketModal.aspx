<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasketModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.BasketModal" %>

<!DOCTYPE html>
<html>
<head>
    <title>Custom Modal</title>
    <link rel="stylesheet" type="text/css" href="styles.css" />
    <script src="https://cdn.jsdelivr.net/npm/vue@2.6.14"></script>
</head>
<body>
<div id="app">
    <!-- Button to open modal -->
    <button @click="openModal">Add</button>

    <!-- Modal window -->
    <div v-if="isModalOpen" class="modal">
        <div class="modal-content">
            <span class="close" @click="closeModal">&times;</span>
            <h2>Item Tree</h2>
            <ul>
                <li v-for="item in items" :key="item.id">
                    {{ item.name }}
                    <ul>
                        <li v-for="child in item.children" :key="child.id">{{ child.name }}</li>
                    </ul>
                </li>
            </ul>
            <button @click="addItem">Add Item</button>
        </div>
    </div>
</div>
<script src="app.js"></script>
</body>
</html>

