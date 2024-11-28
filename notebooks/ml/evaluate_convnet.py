import pandas as pd
from sklearn.metrics import confusion_matrix, accuracy_score, precision_score, recall_score, f1_score
import seaborn as sns
import matplotlib.pyplot as plt

# Load data from the CSV file
# Make sure to replace the path with your actual CSV file path
# df = pd.read_csv('sampleData.csv')
df = pd.read_csv('sampleData_with_variable_fruit.csv')

# Display the first few rows of the file
print(df.head())

# The 'FruitGiven' column contains the actual fruit, and 'FruitRecognized' contains the predicted fruit
y_true = df['FruitGiven']
y_pred = df['FruitRecognized']

# 1. Generate the Confusion Matrix
cm = confusion_matrix(y_true, y_pred, labels=['apple', 'orange', 'pear'])

# Display the Confusion Matrix
print("Confusion Matrix:")
print(cm)

# 2. Visualize the Confusion Matrix using Seaborn for better readability
plt.figure(figsize=(6, 5))
sns.heatmap(cm, annot=True, fmt='d', cmap='Blues', xticklabels=['apple', 'orange', 'pear'], yticklabels=['apple', 'orange', 'pear'])
plt.title('Confusion Matrix')
plt.xlabel('Classified fruit (by convNet)')
plt.ylabel('Actual fruit (leaving the feeder)')
plt.show()

# 3. Calculate Accuracy
accuracy = accuracy_score(y_true, y_pred)
print(f"Accuracy: {accuracy:.2f}")

# 4. Calculate Precision, Recall, and F1-Score for each class
precision = precision_score(y_true, y_pred, average=None, labels=['apple', 'orange', 'pear'])
recall = recall_score(y_true, y_pred, average=None, labels=['apple', 'orange', 'pear'])
f1 = f1_score(y_true, y_pred, average=None, labels=['apple', 'orange', 'pear'])

# 5. Display metrics for each class in table format
class_metrics = pd.DataFrame({
    'Class': ['apple', 'orange', 'pear'],
    'Precision': precision,
    'Recall': recall,
    'F1-Score': f1
})
print("\nMetrics for each class:")
print(class_metrics)

# 6. Calculate the average metrics (Macro, Micro, and Weighted)
precision_macro = precision_score(y_true, y_pred, average='macro')
recall_macro = recall_score(y_true, y_pred, average='macro')
f1_macro = f1_score(y_true, y_pred, average='macro')

precision_micro = precision_score(y_true, y_pred, average='micro')
recall_micro = recall_score(y_true, y_pred, average='micro')
f1_micro = f1_score(y_true, y_pred, average='micro')

precision_weighted = precision_score(y_true, y_pred, average='weighted')
recall_weighted = recall_score(y_true, y_pred, average='weighted')
f1_weighted = f1_score(y_true, y_pred, average='weighted')

# 7. Display average metrics in table format
average_metrics = pd.DataFrame({
    'Metric': ['Precision (Macro)', 'Recall (Macro)', 'F1-Score (Macro)', 
               'Precision (Micro)', 'Recall (Micro)', 'F1-Score (Micro)',
               'Precision (Weighted)', 'Recall (Weighted)', 'F1-Score (Weighted)'],
    'Score': [precision_macro, recall_macro, f1_macro, 
              precision_micro, recall_micro, f1_micro, 
              precision_weighted, recall_weighted, f1_weighted]
})
print("\nAverage Metrics:")
print(average_metrics)
