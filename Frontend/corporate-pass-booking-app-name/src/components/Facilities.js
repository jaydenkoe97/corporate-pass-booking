import React, { useState, useEffect } from 'react';
import { Table } from 'antd';
import axios from 'axios';

const Facilities = () => {
    const [facilities, setFacilities] = useState([]);

    useEffect(() => {
        const fetchFacilities = async () => {
            try {
                const response = await axios.get('https://localhost:7147/api/Facility/FacilityList');
                setFacilities(response.data);
            } catch (err) {
            }
        };

        fetchFacilities();
    }, []);

    const columns = [
        { title: 'Name', dataIndex: 'name', key: 'name' },
        { title: 'Type', dataIndex: 'type', key: 'type' },
        { title: 'Capacity', dataIndex: 'capacity', key: 'capacity' },
        { title: 'Location', dataIndex: 'location', key: 'location' },
        { title: 'Amenities', dataIndex: 'amenities', key: 'amenities' },
    ];

    return (
        <Table dataSource={facilities} columns={columns} rowKey="id" />
    );
};

export default Facilities;