const odbc = require('odbc');


async function example() {
	const connection = await odbc.connect(`Driver=IBM i Access ODBC Driver;System=MYSYSTEM;UID=LALLAN;Password=passwordhere`);
	const result = await connection.query('SELECT * FROM QIWS.QCUSTCDT');
	console.log(result);
}

example();

