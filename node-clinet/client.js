const { Socket } = require('phoenix-channels');
const socket = new Socket('ws://localhost:4000/socket');

socket.connect();

const channel = socket.channel('room:lobby', {});

channel.join()
        .receive('ok', resp => console.log('joining channel : ', channel.topic, ' ', resp))
        .receive('error', resp => console.error('Error joining channel : ', resp));

channel.on('new_msg', payload => {
  console.log('メッセージ通知 : ', payload);
})

channel.push('new_msg', {body: 'Hello World'});