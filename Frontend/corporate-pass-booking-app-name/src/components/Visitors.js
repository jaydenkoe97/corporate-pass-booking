import React, { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Input } from 'antd';
import axios from 'axios';

const Visitors = () => {
    const [visitors, setVisitors] = useState([]);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [editingVisitor, setEditingVisitor] = useState(null);
    const [form] = Form.useForm();

    useEffect(() => {
        const fetchVisitors = async () => {
            try {
                const response = await axios.get('https://localhost:7147/api/Visitor/VisitorList');
                setVisitors(response.data);
            } catch (err) {
                console.error('Error fetching visitors:', err);
            }
        };

        fetchVisitors();
    }, []);

    const showModal = () => {
        setIsModalVisible(true);
        form.resetFields();
    };

    const handleOk = async () => {
        try {
            const values = await form.validateFields();
            if (editingVisitor) {
                await axios.put(`https://localhost:7147/api/Visitor/${editingVisitor.id}`, values);
                setVisitors(visitors.map(v => (v.id === editingVisitor.id ? { ...v, ...values } : v)));
            } else {
                const response = await axios.post('https://localhost:7147/api/Visitor', values);
                setVisitors([...visitors, response.data]);
            }
            setIsModalVisible(false);
            setEditingVisitor(null);
        } catch (error) {
            console.error('Error handling form submission:', error);
        }
    };

    const handleEdit = (visitor) => {
        setEditingVisitor(visitor);
        form.setFieldsValue(visitor);
        setIsModalVisible(true);
    };

    const columns = [
        { title: 'Name', dataIndex: 'name', key: 'name' },
        { title: 'Email', dataIndex: 'email', key: 'email' },
        { title: 'Phone', dataIndex: 'phoneNumber', key: 'phoneNumber' },
        {
            title: 'Action',
            key: 'action',
            render: (_, record) => (
                <Button onClick={() => handleEdit(record)}>Edit</Button>
            ),
        },
    ];

    return (
        <>
            <Button type="primary" onClick={showModal} style={{ marginBottom: '16px' }}>
                Create Visitor
            </Button>
            <Table dataSource={visitors} columns={columns} rowKey="id" />

            <Modal
                title={editingVisitor ? "Edit Visitor" : "New Visitor"}
                visible={isModalVisible}
                onOk={handleOk}
                onCancel={() => setIsModalVisible(false)}
            >
                <Form form={form} layout="vertical">
                    <Form.Item name="name" label="Name" rules={[{ required: true }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="email" label="Email" rules={[{ required: true, type: 'email' }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="phoneNumber" label="Phone" rules={[{ required: true }]}>
                        <Input />
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
};

export default Visitors;