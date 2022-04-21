## ioCode.Serialization

C# types serialization library.<br />
Serializes and deserializes objects into and from XML documents.

### Classes

ioCode.Serialization library has two serialization classes<br />
- **XmlSerializer\<T\>** Xml serialization object to serialize custom types<br />
- **BinSerializer\<T\>** Bin serialization object for XML-based serialization of custom types. The serialized output file is encrypted.

### XmlSerializer\<T\>

#### Write to file
```
  Product product = new Product();
  product.Name = "Product1";
  bool isSuccess = XmlSerializer<Product>.WriteFile(@"C:\users\[user]\documents\product.xml", product);
  MessageBox.Show(isSuccess ? "Success" : "Fail");
```
#### Read from file
```
  Product product = XmlSerializer<Product>.ReadFile(@"C:\users\[user]\documents\product.xml");
  MessageBox.Show((product != null) ? "Success" : "Fail");
```
  
### BinSerializer\<T\>

#### Write to file
```
  Product product = new Product();
  product.Name = "Product1";
  bool isSuccess = BinSerializer<Product>.WriteFile(@"C:\users\[user]\documents\product.bin", "password123", product);
  MessageBox.Show(isSuccess ? "Success" : "Fail");
```
#### Read from file
```
  Product product = BinSerializer<Product>.ReadFile(@"C:\users\[user]\documents\product.bin", "password123");
  MessageBox.Show((product != null) ? "Success" : "Fail");
```
