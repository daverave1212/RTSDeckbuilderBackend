import net from 'net';

function connectTo(port, ip, onConnect) {
    const client = new net.Socket();
    client.connect(port, ip, () => {
        console.log('Connected to server');
        onConnect({
            client,
            send: function(obj) {
                client.write(JSON.stringify(obj))
            },
            onReceive: function(callbackOnObj) {
                client.on('data', data => {
                    console.log(data)
                    callbackOnObj(data)
                })
            },
            onClose: function(callback) {
                client.on('close', callback)
            }
        })
    });

}

connectTo(8080, '127.0.0.1', client => {
    client.onReceive(obj => {
        
    })
    client.send({
        name: "HelloWorld",
        data: [{
            "data": "Just some data mate",
            "gorgo": "danc"
        }]
    })
})





// setTimeout(() => {
//     const client2 = new net.Socket();
//     client2.connect(8080, '127.0.0.1', () => {
//         console.log('Connected to server');
//         setInterval(() => {
//             client2.write('    BOBR KURWA!!!');
//         }, 500)
//     });
//     client2.on('data', (data) => {
//         console.log('    (BOBR) Received:', data.toString());
//     });
//     client2.on('close', () => {});
// }, 1500)