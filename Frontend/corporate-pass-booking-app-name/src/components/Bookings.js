import React, { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Select, DatePicker } from 'antd';
import axios from 'axios';
import moment from 'moment';

const { Option } = Select;

const Bookings = () => {
    const [visitors, setVisitors] = useState([]);
    const [facilities, setFacilities] = useState([]);
    const [bookings, setBookings] = useState([]);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [editingBooking, setEditingBooking] = useState(null);
    const [form] = Form.useForm();

    useEffect(() => {
        const fetchBookings = async () => {
            try {
                const response = await axios.get('https://localhost:7147/api/Booking/BookingList');
                const bookingsWithDates = response.data.map(booking => ({
                    ...booking,
                    bookingDate: booking.bookingDate ? moment(booking.bookingDate) : null
                }));
                setBookings(bookingsWithDates);
            } catch (err) {
                console.error('Error fetching bookings:', err);
            }
        };

        const fetchVisitors = async () => {
            try {
                const response = await axios.get('https://localhost:7147/api/Visitor/VisitorList');
                setVisitors(response.data);
            } catch (err) {
                console.error('Error fetching visitors:', err);
            }
        };

        const fetchFacilities = async () => {
            try {
                const response = await axios.get('https://localhost:7147/api/Facility/FacilityList');
                setFacilities(response.data);
            } catch (err) {
                console.error('Error fetching facilities:', err);
            }
        };

        fetchFacilities();
        fetchBookings();
        fetchVisitors();
    }, []);

    const showModal = () => {
        setIsModalVisible(true);
        form.resetFields();
    };

    const handleOk = async () => {
        try {
            const values = await form.validateFields();
            values.bookingDate = values.bookingDate.toISOString();

            if (editingBooking) {
                await axios.put(`https://localhost:7147/api/Booking/${editingBooking.id}`, values);
                setBookings(bookings.map(b => (b.id === editingBooking.id ? { ...b, ...values, bookingDate: values.bookingDate ? moment(values.bookingDate) : null } : b)));
            } else {
                const response = await axios.post('https://localhost:7147/api/Booking', values);
                setBookings([...bookings, response.data]);
            }
            setIsModalVisible(false);
            setEditingBooking(null);
        } catch (error) {
            console.error('Error handling form submission:', error);
        }
    };

    const handleEdit = (booking) => {
        setEditingBooking(booking);
        form.setFieldsValue({
            ...booking,
            bookingDate: booking.bookingDate ? moment(booking.bookingDate) : null
        });
        setIsModalVisible(true);
    };

    const columns = [
        { title: 'Visitor Name', dataIndex: 'visitorName', key: 'visitorName' },
        { title: 'Facility Name', dataIndex: 'facilityName', key: 'facilityName' },
        {
            title: 'Booking Date',
            dataIndex: 'bookingDate',
            key: 'bookingDate',
            render: date => moment(date).format('MM/DD/YYYY'),
        },
        { title: 'Status', dataIndex: 'status', key: 'status' },
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
                Create Booking
            </Button>
            <Table dataSource={bookings} columns={columns} rowKey="id" />

            <Modal
                title={editingBooking ? "Edit Booking" : "New Booking"}
                visible={isModalVisible}
                onOk={handleOk}
                onCancel={() => setIsModalVisible(false)}
            >
                <Form form={form} layout="vertical">
                    <Form.Item name="visitorName" label="Visitor" rules={[{ required: true }]}>
                        <Select placeholder="Select a visitor">
                            {visitors.map(visitor => (
                                <Option key={visitor.id} value={visitor.name}>{visitor.name}</Option>
                            ))}
                        </Select>
                    </Form.Item>
                    <Form.Item name="facilityName" label="Facility" rules={[{ required: true }]}>
                        <Select placeholder="Select a facility">
                            {facilities.map(facility => (
                                <Option key={facility.id} value={facility.name}>{facility.name}</Option>
                            ))}
                        </Select>
                    </Form.Item>
                    <Form.Item name="bookingDate" label="Booking Date" rules={[{ required: true }]}>
                        <DatePicker format="MM/DD/YYYY" />
                    </Form.Item>
                    <Form.Item name="status" label="Status" rules={[{ required: true }]}>
                        <Select placeholder="Select status">
                            <Option value="Pending">Pending</Option>
                            <Option value="Confirmed">Confirmed</Option>
                            <Option value="Cancelled">Cancelled</Option>
                        </Select>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
};

export default Bookings;