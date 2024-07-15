new Vue({
    el: '#app',
    data: {
        isModalOpen: false,
        items: [
            {
                id: 1,
                name: 'Item 1',
                children: [
                    { id: 11, name: 'Child 1.1' },
                    { id: 12, name: 'Child 1.2' }
                ]
            },
            {
                id: 2,
                name: 'Item 2',
                children: [
                    { id: 21, name: 'Child 2.1' },
                    { id: 22, name: 'Child 2.2' }
                ]
            }
        ]
    },
    methods: {
        openModal() {
            this.isModalOpen = true;
        },
        closeModal() {
            this.isModalOpen = false;
        },
        addItem() {
            // Добавить новый элемент логика
            alert('Add Item clicked');
        }
    }
});