import React, { useState } from 'react';
import { Layout, Menu } from 'antd';
import Visitors from './components/Visitors';
import Facilities from './components/Facilities';
import Bookings from './components/Bookings';

const { Header, Content, Sider } = Layout;

const App = () => {
    const [selectedMenu, setSelectedMenu] = useState('Visitors');

    const renderContent = () => {
        switch (selectedMenu) {
            case 'Visitors':
                return <Visitors />;
            case 'Facilities':
                return <Facilities />;
            case 'Bookings':
                return <Bookings />;
            default:
                return null;
        }
    };

    return (
        <Layout style={{ minHeight: '100vh' }}>
            <Sider width={200}>
                <Menu
                    mode="inline"
                    selectedKeys={[selectedMenu]}
                    style={{ height: '100%', borderRight: 0 }}
                >
                    <Menu.Item key="Visitors" onClick={() => setSelectedMenu('Visitors')}>
                        Visitors
                    </Menu.Item>
                    <Menu.Item key="Facilities" onClick={() => setSelectedMenu('Facilities')}>
                        Facilities
                    </Menu.Item>
                    <Menu.Item key="Bookings" onClick={() => setSelectedMenu('Bookings')}>
                        Bookings
                    </Menu.Item>
                </Menu>
            </Sider>
            <Layout>
                <Header style={{ background: '#fff', padding: '0px', margin: '5px' }}>
                    <h1 style={{ margin: '0px', paddingLeft: '10px', fontSize: '24px', fontWeight: 'bold' }}>
                        {selectedMenu}
                    </h1>
                </Header>
                <Content style={{ margin: '16px' }}>
                    {renderContent()}
                </Content>
            </Layout>
        </Layout>
    );
};

export default App;